using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket.RejectTransfer
{
    public partial class RejectTransferTicket
    {

        public class Handler : IRequestHandler<RejectTransferTicketCommand, Result>
        {

            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(RejectTransferTicketCommand command, CancellationToken cancellationToken)
            {
                var userDetails = await _context.Users
                    .FirstOrDefaultAsync(x => x.Id == command.Transacted_By);

                var transferTicketExist = await _context.TransferTicketConcerns
                    .Include(t => t.TicketConcern)                 
                    .FirstOrDefaultAsync(x => x.Id == command.TransferTicketId, cancellationToken);

                if (transferTicketExist is null)
                    return Result.Failure(TransferTicketError.TransferTicketConcernIdNotExist());

                if(transferTicketExist.IsActive is false)
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
                    .Where(x => x.TransferTicketConcernId == transferTicketExist.Id)
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

                await UpdateRejectConcernStatus(approverUserList, transferTicketExist,command,cancellationToken);

                await TransferHistory(userDetails, transferTicketExist, command, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }


            private async Task UpdateRejectConcernStatus(List<ApproverTicketing> approverTicketing,TransferTicketConcern transferTicketConcern, RejectTransferTicketCommand command,CancellationToken cancellationToken)
            {
                transferTicketConcern.IsActive = false;
                transferTicketConcern.IsRejectTransfer = true;
                transferTicketConcern.RejectTransferBy = command.RejectTransfer_By;
                transferTicketConcern.RejectTransferAt = DateTime.Now;
                transferTicketConcern.RejectRemarks = command.Reject_Remarks;

                var ticketConcernExist = await _context.TicketConcerns
                    .FirstOrDefaultAsync(x => x.Id == transferTicketConcern.TicketConcernId);

                ticketConcernExist.IsTransfer = null;
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

            private async Task TransferHistory(User user,TransferTicketConcern transferTicketConcern, RejectTransferTicketCommand command, CancellationToken cancellationToken)
            {
                var addTicketHistory = new TicketHistory
                {
                    TicketConcernId = transferTicketConcern.TicketConcernId,
                    TransactedBy = transferTicketConcern.TransferBy,
                    TransactionDate = DateTime.Now,
                    Request = TicketingConString.Reject,
                    Status = $"{TicketingConString.TransferReject} {user.Fullname}",
                    Remarks = command.Reject_Remarks
                };

                await _context.TicketHistories.AddAsync(addTicketHistory, cancellationToken);

                var addNewTicketTransactionNotification = new TicketTransactionNotification
                {

                    Message = $"Transfer request for ticket number {transferTicketConcern.TicketConcernId} was rejected.",
                    AddedBy = user.Id,
                    Created_At = DateTime.Now,
                    ReceiveBy = transferTicketConcern.TicketConcern.UserId.Value,
                    Modules = PathConString.IssueHandlerConcerns,
                    Modules_Parameter = PathConString.OpenTicket,
                    PathId = transferTicketConcern.TicketConcernId,

                };

                await _context.TicketTransactionNotifications.AddAsync(addNewTicketTransactionNotification);

            }


        }
    }
}
