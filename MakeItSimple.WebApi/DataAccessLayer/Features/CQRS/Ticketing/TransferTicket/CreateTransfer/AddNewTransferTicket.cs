using DocumentFormat.OpenXml.InkML;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models;
using MakeItSimple.WebApi.Models.Setup.ApproverSetup;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket.CreateTransfer
{
    public partial class AddNewTransferTicket
    {

        public class Handler : IRequestHandler<AddNewTransferTicketCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(AddNewTransferTicketCommand command, CancellationToken cancellationToken)
            {

                var userDetails = await _context.Users
                    .FirstOrDefaultAsync(x => x.Id == command.Added_By, cancellationToken);

                var transferToDetails = await _context.Users
                    .FirstOrDefaultAsync(t => t.Id == command.Transfer_To, cancellationToken);

                var ticketConcernExist = await _context.TicketConcerns
                    .FirstOrDefaultAsync(x => x.Id == command.TicketConcernId, cancellationToken);


               var validation = await ValidationHandler(ticketConcernExist,command, cancellationToken);
                if(validation is not null)
                    return validation;
                
                var transferTicketExist = await _context.TransferTicketConcerns
                        .FirstOrDefaultAsync(x => x.Id == command.TransferTicketId);

                if (transferTicketExist is not null)
                {
                    if (transferTicketExist.IsActive is false)
                        return Result.Failure(TicketRequestError.TicketAlreadyCancel());

                    if (transferTicketExist.IsRejectTransfer is true)
                        return Result.Failure(TicketRequestError.TicketAlreadyReject());

                    await UpdateTransferTicket(transferTicketExist,command ,cancellationToken);
                }
                else
                {

                    var approverList = await _context.Approvers
                        .Include(x => x.User)
                        .Where(x => x.SubUnitId == transferToDetails.SubUnitId)
                        .ToListAsync();

                    if (!approverList.Any())
                        return Result.Failure(ClosingTicketError.NoApproverHasSetup());

                    var approverUser = approverList
                        .First(x => x.ApproverLevel == approverList.Min(x => x.ApproverLevel));

                    var addTransferTicket = await CreateTransferTicket(approverUser,transferTicketExist,ticketConcernExist, command, cancellationToken);

                    transferTicketExist = addTransferTicket;

                    foreach(var approver in approverList)
                    {
                        await CreateApprover(approver,ticketConcernExist, transferTicketExist, command, cancellationToken);
                    }

                    await CreateHistory(approverList, ticketConcernExist, command, cancellationToken);

                    await CreateTransactionNotification(addTransferTicket,ticketConcernExist,userDetails,command,cancellationToken);

                }

                if (!Directory.Exists(TicketingConString.AttachmentPath))
                {
                    Directory.CreateDirectory(TicketingConString.AttachmentPath);
                }

                if (command.AddTransferAttachments.Count(x => x.Attachment != null) > 0)
                {
                    var attachment = await CreateAttachment(transferTicketExist,command,cancellationToken);
                    if(attachment is not null) 
                        return attachment;
 
                }

                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }

            private async Task<Result?> ValidationHandler(TicketConcern ticketConcern, AddNewTransferTicketCommand command, CancellationToken cancellationToken)
            {
                if (ticketConcern is null)
                    return Result.Failure(TransferTicketError.TicketConcernIdNotExist());

                if (ticketConcern.IsTransfer is not null && ticketConcern.IsClosedApprove is not null)
                    return Result.Failure(TransferTicketError.TransferInvalid());

                return null;
            }

            private async Task UpdateTransferTicket (TransferTicketConcern transferTicketConcern, AddNewTransferTicketCommand command, CancellationToken cancellationToken)
            {
                var isChange = false;

                if (transferTicketConcern.TransferRemarks != command.TransferRemarks)
                {
                    transferTicketConcern.TransferRemarks = command.TransferRemarks;
                    isChange = true;
                }

                if (transferTicketConcern.TransferTo != command.Transfer_To)
                {
                    transferTicketConcern.TransferTo = command.Transfer_To;
                    isChange = true;
                }

                if (isChange)
                {
                    transferTicketConcern.ModifiedBy = command.Modified_By;
                    transferTicketConcern.UpdatedAt = DateTime.Now;
                }

            }

            private async Task<TransferTicketConcern> CreateTransferTicket(Approver approver,TransferTicketConcern transferTicketConcern,TicketConcern ticketConcern, AddNewTransferTicketCommand command, CancellationToken cancellationToken)
            {
                ticketConcern.IsTransfer = false;

                var addTransferTicket = new TransferTicketConcern
                {
                    TicketConcernId = ticketConcern.Id,
                    TransferRemarks = command.TransferRemarks,
                    TransferBy = command.Transfer_By,
                    TransferTo = command.Transfer_To,
                    IsTransfer = false,
                    AddedBy = command.Added_By,
                    Current_Target_Date = ticketConcern.TargetDate,
                    TicketApprover = approver.UserId,
                };

                await _context.TransferTicketConcerns.AddAsync(addTransferTicket);
                await _context.SaveChangesAsync(cancellationToken);

                return addTransferTicket;

            }

            private async Task CreateApprover(Approver approver, TicketConcern ticketConcern,TransferTicketConcern transferTicketConcern, AddNewTransferTicketCommand command,CancellationToken cancellationToken)
            {
                var addApprover = new ApproverTicketing
                {
                    TicketConcernId = ticketConcern.Id,
                    TransferTicketConcernId = transferTicketConcern.Id,
                    UserId = approver.UserId,
                    ApproverLevel = approver.ApproverLevel,
                    AddedBy = command.Added_By,
                    CreatedAt = DateTime.Now,
                    Status = TicketingConString.Transfer,

                };

                await _context.ApproverTicketings.AddAsync(addApprover, cancellationToken);

            }

            private async Task CreateHistory(List<Approver> approverList,TicketConcern ticketConcern, AddNewTransferTicketCommand command, CancellationToken cancellationToken)
            {
                var addTicketHistory = new TicketHistory
                {

                    TicketConcernId = ticketConcern.Id,
                    TransactedBy = command.Transacted_By,
                    TransactionDate = DateTime.Now,
                    Request = TicketingConString.ForTransfer,
                    Status = TicketingConString.TransferRequest

                };

                await _context.TicketHistories.AddAsync(addTicketHistory, cancellationToken);

                foreach(var approver in approverList)
                {
                    var approverLevel = approver.ApproverLevel == 1 ? $"{approver.ApproverLevel}st"
                        : approver.ApproverLevel == 2 ? $"{approver.ApproverLevel}nd"
                        : approver.ApproverLevel == 3 ? $"{approver.ApproverLevel}rd"
                        : $"{approver.ApproverLevel}th";

                    var addApproverHistory = new TicketHistory
                    {
                        TicketConcernId = ticketConcern.Id,
                        TransactedBy = approver.UserId,
                        TransactionDate = DateTime.Now,
                        Request = TicketingConString.Approval,
                        Status = $"{TicketingConString.TransferForApproval} {approverLevel} Approver",
                        Approver_Level = approver.ApproverLevel,

                    };

                    await _context.TicketHistories.AddAsync(addApproverHistory, cancellationToken);

                }

            }

            private async Task CreateTransactionNotification(TransferTicketConcern transferTicketConcern,TicketConcern ticketConcern, User user, AddNewTransferTicketCommand command, CancellationToken cancellationToken)
            {
                var addNewTicketTransactionNotification = new TicketTransactionNotification
                {

                    Message = $"Ticket number {ticketConcern.Id} is pending for transfer approval",
                    AddedBy = user.Id,
                    Created_At = DateTime.Now,
                    ReceiveBy = transferTicketConcern.TicketApprover.Value,
                    Modules = PathConString.Approval,
                    Modules_Parameter = PathConString.ForTransfer,
                    PathId = ticketConcern.Id,

                };

                await _context.TicketTransactionNotifications.AddAsync(addNewTicketTransactionNotification);

            }

            private async Task<Result?> CreateAttachment(TransferTicketConcern transferTicketConcern, AddNewTransferTicketCommand command, CancellationToken cancellationToken)
            {
                foreach (var attachments in command.AddTransferAttachments.Where(a => a.Attachment.Length > 0))
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
                            TicketConcernId = command.TicketConcernId,
                            TransferTicketConcernId = transferTicketConcern.Id,
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
