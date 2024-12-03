using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models.Setup.ApproverSetup;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.OnHoldTicket.CreateOnHold
{
    public partial class CreateOnHoldTicket
    {

        public class Handler : IRequestHandler<CreateOnHoldTicketCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(CreateOnHoldTicketCommand command, CancellationToken cancellationToken)
            {
                var onHoldConcern = new List<TicketOnHold>();

                var userDetails = await _context.Users
                    .FirstOrDefaultAsync(x => x.Id == command.Added_By, cancellationToken);

                var ticketConcernExist = await _context.TicketConcerns
                    .Include(i => i.RequestorByUser)
                    .FirstOrDefaultAsync(t => t.Id == command.TicketConcernId,cancellationToken);

                if (ticketConcernExist is null)
                    return Result.Failure(TicketRequestError.TicketConcernIdNotExist());

                var onHoldExist = await _context.TicketOnHolds
                    .FirstOrDefaultAsync(o => o.Id == command.Id,cancellationToken);

                if (onHoldExist is not null)
                {
                    if (onHoldExist.IsActive is false)
                        return Result.Failure(TicketRequestError.TicketAlreadyCancel());

                    if (onHoldExist.IsRejectOnHold is true)
                        return Result.Failure(TicketRequestError.TicketAlreadyReject());

                    onHoldExist.Reason = command.Reason;
                    ticketConcernExist.OnHoldReason = command.Reason;

                }
                else
                {
                    var approverList = await _context.Approvers
                        .Include(x => x.User)
                        .Where(x => x.SubUnitId == userDetails.SubUnitId)
                        .ToListAsync();

                    if (!approverList.Any())
                        return Result.Failure(ClosingTicketError.NoApproverHasSetup());

                    var approverUser = approverList
                        .First(x => x.ApproverLevel == approverList.Min(x => x.ApproverLevel));

                    var addOnHold = await CreateOnHold(approverUser,ticketConcernExist,command, cancellationToken);
                    onHoldConcern.Add(addOnHold);
                    onHoldExist = addOnHold;

                    foreach( var approver in approverList)
                    {
                        await CreateApprover(approver, ticketConcernExist, onHoldExist, command, cancellationToken);
                    }

                    await OnHoldTicketHistory(approverList,ticketConcernExist,command, cancellationToken);
                    await TransactionNotification(onHoldExist,ticketConcernExist, command, cancellationToken);

                }

                if (!Directory.Exists(TicketingConString.AttachmentPath))
                {
                    Directory.CreateDirectory(TicketingConString.AttachmentPath);
                }

                if (command.OnHoldAttachments.Count(x => x.Attachment != null) > 0)
                {

                    var attachmentValidation = await AttachmentHandler(onHoldConcern, command, cancellationToken);
                    if (attachmentValidation is not null)
                        return attachmentValidation;
                }

                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }

            private async Task CreateApprover(Approver approver, TicketConcern ticketConcern, TicketOnHold ticketOnHold, CreateOnHoldTicketCommand command, CancellationToken cancellationToken)
            {
                var addApprover = new ApproverTicketing
                {
                    TicketConcernId = command.TicketConcernId,
                    TicketOnHoldId = ticketOnHold.Id,
                    UserId = approver.UserId,
                    ApproverLevel = approver.ApproverLevel,
                    AddedBy = command.Added_By,
                    CreatedAt = DateTime.Now,
                    Status = TicketingConString.OnHold,

                };

                await _context.ApproverTicketings.AddAsync(addApprover, cancellationToken);

            }

            private async Task<TicketOnHold> CreateOnHold(Approver approver,TicketConcern ticketConcern, CreateOnHoldTicketCommand command , CancellationToken cancellationToken)
            {
                ticketConcern.OnHold = false;

                var addOnHold = new TicketOnHold
                {
                   TicketConcernId = command.TicketConcernId,
                   Reason = command.Reason,
                   AddedBy = command.Added_By,
                   IsHold = false, 
                   TicketApprover = approver.UserId,

                };

                await _context.TicketOnHolds.AddAsync(addOnHold);

                await _context.SaveChangesAsync(cancellationToken); 
                return addOnHold;

            }

            private async Task OnHoldTicketHistory(List<Approver> approverList, TicketConcern ticketConcern, CreateOnHoldTicketCommand command , CancellationToken cancellationToken)
            {
                var addTicketHistory = new TicketHistory
                {
                    TicketConcernId = command.TicketConcernId,
                    TransactedBy = command.Added_By,
                    TransactionDate = DateTime.Now,
                    Request = TicketingConString.OnHold,
                    Status = TicketingConString.OnHoldRequest

                };

                await _context.TicketHistories.AddAsync(addTicketHistory,cancellationToken);


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
                        Status = $"{TicketingConString.OnHoldForApproval} {approverLevel} Approver",
                        Approver_Level = approver.ApproverLevel,

                    };

                    await _context.TicketHistories.AddAsync(addApproverHistory, cancellationToken);

                }

            }

            private async Task<TicketTransactionNotification> TransactionNotification(TicketOnHold onHoldTicket,TicketConcern ticketConcern, CreateOnHoldTicketCommand command, CancellationToken cancellationToken)
            {
                 var addNewTicketTransactionNotification = new TicketTransactionNotification
                {

                    Message = $"Ticket number {command.TicketConcernId} is pending for on-hold approval",
                    AddedBy = command.Added_By.Value,
                    Created_At = DateTime.Now,
                    Modules = PathConString.Approval,
                    Modules_Parameter = PathConString.ForOnHold,
                    ReceiveBy = onHoldTicket.TicketApprover.Value,
                    PathId = command.TicketConcernId,

                };

                await _context.TicketTransactionNotifications.AddAsync(addNewTicketTransactionNotification);

                return addNewTicketTransactionNotification;
            }
            private async Task<Result?> AttachmentHandler(List<TicketOnHold> onHold, CreateOnHoldTicketCommand command, CancellationToken cancellationToken)
            {

                foreach (var attachments in command.OnHoldAttachments.Where(a => a.Attachment.Length > 0))
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

                    if (ticketAttachment is not null)
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
                            TicketOnHoldId = onHold.First().Id,
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
