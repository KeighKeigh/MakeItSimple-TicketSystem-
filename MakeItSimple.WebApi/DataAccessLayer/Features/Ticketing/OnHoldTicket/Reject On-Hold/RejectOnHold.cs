using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket.RejectTransfer.RejectTransferTicket;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.OnHoldTicket.Reject_On_Hold
{
    public partial class RejectOnHold
    {

        public class Handler : IRequestHandler<RejectOnHoldCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async  Task<Result> Handle(RejectOnHoldCommand command, CancellationToken cancellationToken)
            {
                var userDetails = await _context.Users
                    .FirstOrDefaultAsync(x => x.Id == command.Transacted_By);

                var onHoldTicketExist = await _context.TicketOnHolds
                    .Include(t => t.TicketConcern)
                    .FirstOrDefaultAsync(x => x.Id == command.OnHoldTicketId, cancellationToken);

                if (onHoldTicketExist is null)
                    return Result.Failure(TicketOnHoldError.TicketOnHoldIdNotExist());

                if (onHoldTicketExist.IsActive is false)
                    return Result.Failure(TicketRequestError.TicketAlreadyCancel());

                var userRoleList =
                    await _context.UserRoles
                    .Select(u => new
                    {
                        u.Permissions,
                        u.UserRoleName,
                    })
                    .ToListAsync();

                var approverUserList = await _context.ApproverTicketings
                    .Where(x => x.TicketOnHoldId == onHoldTicketExist.Id)
                    .ToListAsync();

                if (!approverUserList.Any())
                {
                    return Result.Failure(TransferTicketError.NoApproverExist());
                }

                var approverPermission = userRoleList
                .Where(x => x.Permissions.Contains(TicketingConString.Approver))
                .Select(x => x.UserRoleName);

                if (!approverPermission.Any(x => x.Contains(command.Role)))
                {
                    return Result.Failure(TicketRequestError.NotAutorize());
                }

                await UpdateRejectConcernStatus(approverUserList, onHoldTicketExist, command, cancellationToken);
                await OnHoldHistory(userDetails, onHoldTicketExist, command, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }


            private async Task UpdateRejectConcernStatus(List<ApproverTicketing> approverTicketing, TicketOnHold ticketOnHold, RejectOnHoldCommand command, CancellationToken cancellationToken)
            {
                ticketOnHold.IsActive = false;
                ticketOnHold.IsRejectOnHold = true;
                ticketOnHold.RejectOnHoldBy = command.RejectOnHold_By;
                ticketOnHold.RejectOnHoldAt = DateTime.Now;
                ticketOnHold.RejectRemarks = command.Reject_Remarks;

                var ticketConcernExist = await _context.TicketConcerns
                    .FirstOrDefaultAsync(x => x.Id == ticketOnHold.TicketConcernId);

                ticketConcernExist.OnHold = null;
                ticketConcernExist.Remarks = command.Reject_Remarks;

                foreach (var approverUserId in approverTicketing)
                {
                    _context.Remove(approverUserId);
                }

                var ticketHistory = await _context.TicketHistories
                    .Where(x => x.TicketConcernId == ticketConcernExist.Id
                     && x.IsApprove == null && x.Request.Contains(TicketingConString.Approval)
                     || x.Request.Contains(TicketingConString.NotConfirm))
                    .ToListAsync();

                foreach (var item in ticketHistory)
                {
                    _context.TicketHistories.Remove(item);
                }

            }

            private async Task OnHoldHistory(User user, TicketOnHold ticketOnHold, RejectOnHoldCommand command, CancellationToken cancellationToken)
            {
                var addTicketHistory = new TicketHistory
                {
                    TicketConcernId = ticketOnHold.TicketConcernId,
                    TransactedBy = command.Transacted_By,
                    TransactionDate = DateTime.Now,
                    Request = TicketingConString.Reject,
                    Status = $"{TicketingConString.OnHoldReject} {user.Fullname}",
                    Remarks = command.Reject_Remarks
                };

                await _context.TicketHistories.AddAsync(addTicketHistory, cancellationToken);


                var addNewTicketTransactionNotification = new TicketTransactionNotification
                {

                    Message = $"On-hold request for ticket number {ticketOnHold.TicketConcernId} was rejected.",
                    AddedBy = user.Id,
                    Created_At = DateTime.Now,
                    ReceiveBy = ticketOnHold.TicketConcern.UserId.Value,
                    Modules = PathConString.IssueHandlerConcerns,
                    Modules_Parameter = PathConString.OpenTicket,
                    PathId = ticketOnHold.TicketConcernId.Value,

                };

                await _context.TicketTransactionNotifications.AddAsync(addNewTicketTransactionNotification);

            }


        }
    }
}
