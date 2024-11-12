using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Cloudinary;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosedTicketConcern.ReturnClosed
{
    public partial class ReturnClosedTicket
    {

        public class Handler : IRequestHandler<ReturnClosedTicketCommand, Result>
        {
            private readonly MisDbContext _context;
            private readonly Cloudinary _cloudinary;
            private readonly TransformUrl _url;

            public Handler(MisDbContext context, IOptions<CloudinaryOption> options, TransformUrl url)
            {
                _context = context;
                var account = new Account(
                    options.Value.Cloudname,
                    options.Value.ApiKey,
                    options.Value.ApiSecret
                    );
                _cloudinary = new Cloudinary(account);
                _url = url;
            }

            public async Task<Result> Handle(ReturnClosedTicketCommand command, CancellationToken cancellationToken)
            {

                var userDetails = await _context.Users
                   .FirstOrDefaultAsync(x => x.Id == command.Added_By, cancellationToken);

                var requestConcernExist = await _context.RequestConcerns
                    .FirstOrDefaultAsync(x => x.Id == command.RequestConcernId, cancellationToken);

                var validation = await TicketValidation(requestConcernExist,command ,cancellationToken);
                if (validation is not null)
                    return validation;

                await UpdateRequestConcernStatus(requestConcernExist,command,cancellationToken);
                await RequestConcernHistory(userDetails,requestConcernExist, command, cancellationToken);


                if (!Directory.Exists(TicketingConString.AttachmentPath))
                {
                    Directory.CreateDirectory(TicketingConString.AttachmentPath);
                }

                if (command.ReturnTicketAttachments.Count(x => x.Attachment != null) > 0)
                {
                    var attachments = await TicketAttachments(requestConcernExist,command, cancellationToken);
                    if(attachments is not null) 
                        return attachments;
                }

                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }



            private async Task UpdateRequestConcernStatus(RequestConcern requestConcern, ReturnClosedTicketCommand command, CancellationToken cancellationToken)
            {
                requestConcern.ConcernStatus = TicketingConString.OnGoing;
                requestConcern.IsDone = false;

                var ticketConcernExist = await _context.TicketConcerns
                     .FirstOrDefaultAsync(x => x.RequestConcernId == requestConcern.Id, cancellationToken);

                ticketConcernExist.IsClosedApprove = null;
                ticketConcernExist.Closed_At = null;
                ticketConcernExist.ClosedApproveBy = null;
                ticketConcernExist.ConcernStatus = TicketingConString.OnGoing;
                ticketConcernExist.IsDone = null;

                var ticketHistory = await _context.TicketHistories
                    .Where(x => x.TicketConcernId == ticketConcernExist.Id
                    && x.Request.Contains(TicketingConString.NotConfirm))
                    .FirstOrDefaultAsync();

                _context.TicketHistories.Remove(ticketHistory);

            }

            private async Task RequestConcernHistory(User user, RequestConcern requestConcern, ReturnClosedTicketCommand command, CancellationToken cancellationToken)
            {
                var addTicketHistory = new TicketHistory
                {
                    TicketConcernId = requestConcern.Id,
                    TransactedBy = command.Added_By,
                    TransactionDate = DateTime.Now,
                    Request = TicketingConString.Disapprove,
                    Status = TicketingConString.CloseReturn,
                    Remarks = command.Remarks,
                };

                await _context.TicketHistories.AddAsync(addTicketHistory, cancellationToken);

                var addNewTicketTransactionNotification = new TicketTransactionNotification
                {

                    Message = $"Confirmation request for ticket number {requestConcern.TicketConcerns.First().Id} was rejected.",
                    AddedBy = user.Id,
                    Created_At = DateTime.Now,
                    ReceiveBy = requestConcern.TicketConcerns.First().UserId.Value,
                    Modules = PathConString.IssueHandlerConcerns,
                    Modules_Parameter = PathConString.OpenTicket,
                    PathId = requestConcern.TicketConcerns.First().Id

                };

                await _context.TicketTransactionNotifications.AddAsync(addNewTicketTransactionNotification);

            }

            private async Task<Result?> TicketValidation(RequestConcern requestConcern, ReturnClosedTicketCommand command, CancellationToken cancellationToken)
            {
                if (requestConcern is null)
                    return Result.Failure(TicketRequestError.RequestConcernIdNotExist());

                if (requestConcern.Is_Confirm is true)
                    return Result.Failure(TicketRequestError.TicketAlreadyApproved());


                return null;
            }

            private async Task<Result?> TicketAttachments(RequestConcern requestConcern, ReturnClosedTicketCommand command, CancellationToken cancellationToken)
            {
                foreach (var attachments in command.ReturnTicketAttachments.Where(a => a.Attachment.Length > 0))
                {

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

                    var fileName = $"{Guid.NewGuid()}{extension}";
                    var filePath = Path.Combine(TicketingConString.AttachmentPath, fileName);

                    var ticketAttachment = await _context.TicketAttachments
                        .FirstOrDefaultAsync(x => x.Id == attachments.TicketAttachmentId, cancellationToken);

                    if (ticketAttachment != null)
                    {
                        ticketAttachment.Attachment = filePath;
                        ticketAttachment.FileName = attachments.Attachment.FileName;
                        ticketAttachment.FileSize = attachments.Attachment.Length;
                        ticketAttachment.UpdatedAt = DateTime.Now;

                    }
                    else
                    {
                        var addAttachment = new TicketAttachment
                        {
                            TicketConcernId = requestConcern.TicketConcerns.First().Id,
                            Attachment = filePath,
                            FileName = attachments.Attachment.FileName,
                            FileSize = attachments.Attachment.Length,
                            AddedBy = command.Added_By,
                        };

                        await _context.TicketAttachments.AddAsync(addAttachment);

                    }

                    await using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await attachments.Attachment.CopyToAsync(stream);
                    }
                }

                return null;
            }



        }
    }
}
