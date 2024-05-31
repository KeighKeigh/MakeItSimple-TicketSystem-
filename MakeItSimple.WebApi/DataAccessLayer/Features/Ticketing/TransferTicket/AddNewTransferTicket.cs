﻿using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket.AddNewTransferTicket.AddNewTransferTicketCommand;
using MakeItSimple.WebApi.Common.Cloudinary;
using Microsoft.Extensions.Options;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket
{
    public class AddNewTransferTicket
    {
        public class AddNewTransferTicketCommand : IRequest<Result>
        {
            public Guid ? Added_By { get; set; }
            public Guid ? Modified_By { get; set; }
            public Guid ? Requestor_By { get; set; }
            public Guid ? Transfer_By { get; set; }
            public string Remarks { get; set; }
            public int? TransferTicketId { get; set; }
            public int ? TicketConcernId { get; set; }
            public string TransferRemarks { get; set; }
            public List<AddTransferAttachment> AddTransferAttachments { get; set; }

            public class AddTransferAttachment
            {
                public int ? TicketAttachmentId { get; set; }
                public IFormFile Attachment { get; set; }
            }
        }

        public class Handler : IRequestHandler<AddNewTransferTicketCommand, Result>
        {
            private readonly MisDbContext _context;
            private readonly Cloudinary _cloudinary;

            public Handler(MisDbContext context, IOptions<CloudinaryOption> options)
            {
                _context = context;
                var account = new Account(
                    options.Value.Cloudname,
                    options.Value.ApiKey,
                    options.Value.ApiSecret
                    );
                _cloudinary = new Cloudinary(account);
            }

            public async Task<Result> Handle(AddNewTransferTicketCommand command, CancellationToken cancellationToken)
            {

                var updateTransferList = new List<TransferTicketConcern>();
                var updateTicketAttachmentList = new List<TicketAttachment>();

                var userDetails = await _context.Users
                    .FirstOrDefaultAsync(x => x.Id == command.Added_By, cancellationToken);
               
                var ticketConcernExist = await _context.TicketConcerns
                    .FirstOrDefaultAsync(x => x.Id == command.TicketConcernId, cancellationToken);

                if (ticketConcernExist is null)
                {
                    return Result.Failure(TransferTicketError.TicketConcernIdNotExist());
                }

                if (ticketConcernExist.IsTransfer is not null)
                {
                    if (ticketConcernExist.IsReDate is false || ticketConcernExist.IsReTicket is false
                        || ticketConcernExist.IsClosedApprove is not null)
                    {
                        return Result.Failure(TransferTicketError.TransferInvalid());
                    }
                }


                var approverList = await _context.Approvers
               .Include(x => x.User)
               .Where(x => x.SubUnitId == ticketConcernExist.User.SubUnitId)
               .ToListAsync();

                if (approverList == null)
                {
                    return Result.Failure(TransferTicketError.NoApproverExist());
                }

                var transferTicketExist = await _context.TransferTicketConcerns
                        .FirstOrDefaultAsync(x => x.Id == command.TransferTicketId, cancellationToken);

                if (transferTicketExist is not null)
                {
                    var isChange = false;

                    if(transferTicketExist.TransferRemarks != command.TransferRemarks)
                    {
                        transferTicketExist.TransferRemarks = command.TransferRemarks;
                    }

                    if(isChange)
                    {
                        transferTicketExist.ModifiedBy = command.Modified_By;
                        transferTicketExist.UpdatedAt = DateTime.Now;
                    }

                    if (transferTicketExist.IsRejectTransfer is true)
                    {
                        transferTicketExist.Remarks = command.Remarks;
                        updateTransferList.Add(transferTicketExist);
                    }

                }
                else
                {

                    var approverUser = approverList
                        .FirstOrDefault(x => x.ApproverLevel == approverList.Min(x => x.ApproverLevel));

                    ticketConcernExist.IsTransfer = false;

                    var addTransferTicket = new TransferTicketConcern
                    {
                        TicketConcernId = ticketConcernExist.Id,
                        RejectRemarks = command.TransferRemarks,
                        TransferBy = command.Transfer_By,
                        IsTransfer = false,
                        AddedBy = command.Added_By,
                        TicketApprover = approverUser.UserId,

                    };

                    await _context.TransferTicketConcerns.AddAsync(addTransferTicket);

                    await _context.SaveChangesAsync(cancellationToken);

                    foreach (var approver in approverList)
                    {
                        var addNewApprover = new ApproverTicketing
                        {
                            TicketConcernId = ticketConcernExist.Id,
                            TransferTicketConcernId = addTransferTicket.Id,
                            SubUnitId = approver.SubUnitId,
                            UserId = approver.UserId,
                            ApproverLevel = approver.ApproverLevel,
                            AddedBy = command.Added_By,
                            CreatedAt = DateTime.Now,
                            Status = "Transfer",

                        };

                        await _context.ApproverTicketings.AddAsync(addNewApprover, cancellationToken);
                    }

                    var addTicketHistory = new TicketHistory
                    {

                        TicketConcernId = ticketConcernExist.Id,
                        RequestorBy = command.Requestor_By,
                        TransactionDate = DateTime.Now,
                        Request = TicketingConString.Transfer,
                        Status = TicketingConString.RequestCreated

                    };

                    await _context.TicketHistories.AddAsync(addTicketHistory, cancellationToken);

                }

                var uploadTasks = new List<Task>();

                if (command.AddTransferAttachments.Count(x => x.Attachment != null) > 0)
                {

                    foreach (var attachments in command.AddTransferAttachments.Where(attachments => attachments.Attachment.Length > 0))
                    {

                        var ticketAttachment = await _context.TicketAttachments
                         .FirstOrDefaultAsync(x => x.Id == attachments.TicketAttachmentId, cancellationToken);

                        if (attachments.Attachment == null || attachments.Attachment.Length == 0)
                        {
                            return Result.Failure(TicketRequestError.AttachmentNotNull());
                        }

                        if (attachments.Attachment.Length > 10 * 1024 * 1024)
                        {
                            return Result.Failure(TicketRequestError.InvalidAttachmentSize());
                        }

                        var allowedFileTypes = new[] { ".jpeg", ".jpg", ".png", ".docx", ".pdf", ".xlsx" };
                        var extension = Path.GetExtension(attachments.Attachment.FileName)?.ToLowerInvariant();

                        if (extension == null || !allowedFileTypes.Contains(extension))
                        {
                            return Result.Failure(TicketRequestError.InvalidAttachmentType());
                        }

                        uploadTasks.Add(Task.Run(async () =>
                        {
                            await using var stream = attachments.Attachment.OpenReadStream();

                            var attachmentsParams = new RawUploadParams
                            {
                                File = new FileDescription(attachments.Attachment.FileName, stream),
                                PublicId = $"MakeITSimple/Ticketing/Transfer/{userDetails.Fullname}/{attachments.Attachment.FileName}"
                            };

                            var attachmentResult = await _cloudinary.UploadAsync(attachmentsParams);

                            if (ticketAttachment is not null)
                            {

                                var hasChanged = false;

                                if (ticketAttachment.Attachment != attachmentResult.SecureUrl.ToString())
                                {
                                    ticketAttachment.Attachment = attachmentResult.SecureUrl.ToString();
                                    hasChanged = true;
                                }

                                if (hasChanged)
                                {
                                    ticketAttachment.ModifiedBy = command.Modified_By;
                                    ticketAttachment.FileName = attachments.Attachment.FileName;
                                    ticketAttachment.FileSize = attachments.Attachment.Length;
                                    ticketAttachment.UpdatedAt = DateTime.Now;
                                }

                                if(transferTicketExist.IsRejectTransfer is true)
                                {
                                    updateTicketAttachmentList.Add(ticketAttachment);
                                }

                            }
                            else
                            {

                                var addAttachment = new TicketAttachment
                                {
                                    TicketConcernId = ticketConcernExist.Id,
                                    Attachment = attachmentResult.SecureUrl.ToString(),
                                    FileName = attachments.Attachment.FileName,
                                    FileSize = attachments.Attachment.Length,
                                    AddedBy = command.Added_By,
                                };

                                await _context.AddAsync(addAttachment, cancellationToken);

                                if (transferTicketExist is not null &&transferTicketExist.IsRejectTransfer is true )
                                {
                                    updateTicketAttachmentList.Add(ticketAttachment);
                                }
                            }


                        }, cancellationToken));

                    }

                    await Task.WhenAll(uploadTasks);
                }


                if(updateTransferList.Any() || updateTicketAttachmentList.Any())
                {
                    transferTicketExist.IsRejectTransfer = false;
                    transferTicketExist.RejectRemarks = null;
                    transferTicketExist.RejectTransferAt = null;


                    var addTicketHistory = new TicketHistory
                    {
                        TicketConcernId = ticketConcernExist.Id,
                        RequestorBy = command.Requestor_By,
                        TransactionDate = DateTime.Now,
                        Request = TicketingConString.Transfer,
                        Status = TicketingConString.RequestUpdate,

                    };

                    await _context.TicketHistories.AddAsync(addTicketHistory);

                }

                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
        }

    }
}
 