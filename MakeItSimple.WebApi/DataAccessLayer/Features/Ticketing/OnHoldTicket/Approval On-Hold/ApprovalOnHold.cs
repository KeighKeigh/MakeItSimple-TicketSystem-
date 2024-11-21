using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket.ApprovalTransfer.ApprovedTransferTicket;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.OnHoldTicket.Approval_On_Hold
{
    public partial class ApprovalOnHold
    {
        public class Handler : IRequestHandler<ApprovalOnHoldCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(ApprovalOnHoldCommand command, CancellationToken cancellationToken)
            {
                var dateToday = DateTime.Today;

                var userDetails = await _context.Users
                    .FirstOrDefaultAsync(x => x.Id == command.Transacted_By);

                var allUserList = await _context.UserRoles
                    .AsNoTracking()
                    .Select(x => new
                    {
                        x.Id,
                        x.UserRoleName,
                        x.Permissions

                    }).ToListAsync();

                var approverPermissionList = allUserList
                    .Where(x => x.Permissions
                    .Contains(TicketingConString.Approver))
                    .Select(x => x.UserRoleName)
                    .ToList();

                var onHoldTicketExist = await _context.TicketOnHolds
                    .Include(x => x.TicketConcern)
                    .FirstOrDefaultAsync(x => x.Id == command.OnHoldTicketId, cancellationToken);

                if (onHoldTicketExist is null)
                    return Result.Failure(TicketOnHoldError.TicketOnHoldIdNotExist());

                if (onHoldTicketExist.IsActive is false)
                    return Result.Failure(TicketRequestError.TicketAlreadyCancel());

                var onHoldApprover = await _context.ApproverTicketings
                    .Where(x => x.TicketOnHoldId == onHoldTicketExist.Id && x.IsApprove == null)
                    .ToListAsync();

                var ticketHistoryList = await _context.TicketHistories
                    .Where(x => x.TicketConcernId == onHoldTicketExist.TicketConcernId
                     && x.IsApprove == null && x.Request.Contains(TicketingConString.Approval))
                    .ToListAsync();

                var selectOnHoldRequestId = onHoldApprover
                    .FirstOrDefault(x => x.ApproverLevel == onHoldApprover.Min(x => x.ApproverLevel));

                if (selectOnHoldRequestId is not null)
                {
                    if (onHoldTicketExist.TicketApprover != command.Users
                      || !approverPermissionList.Any(x => x.Contains(command.Role)))
                        return Result.Failure(TransferTicketError.ApproverUnAuthorized());

                    selectOnHoldRequestId.IsApprove = true;

                    var userApprovalId = await _context.ApproverTicketings
                        .Where(x => x.TicketOnHoldId == selectOnHoldRequestId.TicketOnHoldId)
                        .ToListAsync();

                    var validateUserApprover = userApprovalId
                        .FirstOrDefault(x => x.ApproverLevel == selectOnHoldRequestId.ApproverLevel + 1);

                    await ApprovalTicketHistory(ticketHistoryList, userDetails, command, cancellationToken);

                    if(validateUserApprover is not null)
                    {
                        await ApprovalTransferNotification(onHoldTicketExist, userDetails, validateUserApprover, command, cancellationToken);

                    }
                    else
                    {
                        await UpdateTransferTicket(onHoldTicketExist, userDetails, command, cancellationToken);
                    }

                }
                else
                {
                    return Result.Failure(TransferTicketError.ApproverUnAuthorized());
                }


                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();

            }
            private async Task ApprovalTicketHistory(List<TicketHistory> ticketHistory, User user, ApprovalOnHoldCommand command, CancellationToken cancellationToken)
            {

                var ticketHistoryApproval = ticketHistory
                    .FirstOrDefault(x => x.Approver_Level != null
                    && x.Approver_Level == ticketHistory.Min(x => x.Approver_Level));

                ticketHistoryApproval.TransactedBy = command.Transacted_By;
                ticketHistoryApproval.TransactionDate = DateTime.Now;
                ticketHistoryApproval.Request = TicketingConString.Approve;
                ticketHistoryApproval.Status = $"{TicketingConString.OnHoldApproved} {user.Fullname}";
                ticketHistoryApproval.IsApprove = true;

            }

            private async Task ApprovalTransferNotification(TicketOnHold ticketOnHold, User user, ApproverTicketing approverTicketing, ApprovalOnHoldCommand command, CancellationToken cancellationToken)
            {
                ticketOnHold.TicketApprover = approverTicketing.UserId;

                var addNewTicketTransactionNotification = new TicketTransactionNotification
                {

                    Message = $"Ticket number {ticketOnHold.TicketConcernId} was approved by {user.Fullname}",
                    AddedBy = user.Id,
                    Created_At = DateTime.Now,
                    ReceiveBy = approverTicketing.UserId.Value,
                    Modules = PathConString.Approval,
                    Modules_Parameter = PathConString.ForTransfer,
                    PathId = ticketOnHold.TicketConcernId.Value

                };

                await _context.TicketTransactionNotifications.AddAsync(addNewTicketTransactionNotification);

                var addTicketApproveNotification = new TicketTransactionNotification
                {

                    Message = $"Ticket number {ticketOnHold.TicketConcernId} was approved by {user.Fullname}",
                    AddedBy = user.Id,
                    Created_At = DateTime.Now,
                    ReceiveBy = ticketOnHold.AddedBy.Value,
                    Modules = PathConString.IssueHandlerConcerns,
                    Modules_Parameter = PathConString.ForOnHold,
                    PathId = ticketOnHold.TicketConcernId.Value,

                };

                await _context.TicketTransactionNotifications.AddAsync(addTicketApproveNotification);

            }

            private async Task UpdateTransferTicket(TicketOnHold ticketOnHold, User user, ApprovalOnHoldCommand command, CancellationToken cancellationToken)
            {
                ticketOnHold.TicketApprover = null;
                ticketOnHold.IsHold = true;

                var ticketConcernExist = await _context.TicketConcerns
                 .Include(x => x.RequestorByUser)
                 .FirstOrDefaultAsync(x => x.Id == ticketOnHold.TicketConcernId);

                ticketConcernExist.OnHold = true;
                ticketConcernExist.Remarks = ticketOnHold.OnHoldRemarks;

                await ApprovedTicketNotification(ticketOnHold, user, command, cancellationToken);

            }

            private async Task ApprovedTicketNotification(TicketOnHold ticketOnHold, User user, ApprovalOnHoldCommand command, CancellationToken cancellationToken)
            {
                var addTransferByTransactionNotification = new TicketTransactionNotification
                {

                    Message = $"Ticket concern number {ticketOnHold.TicketConcernId} has been Hold",
                    AddedBy = user.Id,
                    Created_At = DateTime.Now,
                    ReceiveBy = ticketOnHold.AddedBy.Value,
                    Modules = PathConString.IssueHandlerConcerns,
                    Modules_Parameter = PathConString.OnHold,
                    PathId = ticketOnHold.TicketConcernId.Value,

                };

                await _context.TicketTransactionNotifications.AddAsync(addTransferByTransactionNotification);

                var addTransferToTransactionNotification = new TicketTransactionNotification
                {

                    Message = $"Ticket concern number {ticketOnHold.TicketConcernId} has been Hold",
                    AddedBy = user.Id,
                    Created_At = DateTime.Now,
                    ReceiveBy = ticketOnHold.TicketConcern.RequestorBy.Value,
                    Modules = PathConString.ConcernTickets,
                    Modules_Parameter = PathConString.Ongoing,
                    PathId = ticketOnHold.TicketConcernId.Value,

                };

                await _context.TicketTransactionNotifications.AddAsync(addTransferToTransactionNotification);

            }



        }
    }
}
