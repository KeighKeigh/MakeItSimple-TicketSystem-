using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;

using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket.ApprovalTransfer
{
    public partial class ApprovedTransferTicket
    {

        public class Handler : IRequestHandler<ApprovedTransferTicketCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(ApprovedTransferTicketCommand command, CancellationToken cancellationToken)
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

                var transferTicketExist = await _context.TransferTicketConcerns
                    .Include(x => x.TicketConcern)
                    .FirstOrDefaultAsync(x => x.Id == command.TransferTicketId, cancellationToken);

                if (transferTicketExist is null)
                    return Result.Failure(TransferTicketError.TransferTicketConcernIdNotExist());

                if(transferTicketExist.IsActive is false )
                    return Result.Failure(TicketRequestError.TicketAlreadyCancel());

                var transferApprover = await _context.ApproverTicketings
                    .Where(x => x.TransferTicketConcernId == transferTicketExist.Id && x.IsApprove == null)
                    .ToListAsync();

                var ticketHistoryList = await _context.TicketHistories
                    .Where(x => x.TicketConcernId == transferTicketExist.TicketConcernId
                     && x.IsApprove == null && x.Request.Contains(TicketingConString.Approval))
                    .ToListAsync();

                var selectTransferRequestId = transferApprover
                    .FirstOrDefault(x => x.ApproverLevel == transferApprover.Min(x => x.ApproverLevel));

                if (selectTransferRequestId is not null)
                {

                    if (transferTicketExist.TicketApprover != command.Users
                      || !approverPermissionList.Any(x => x.Contains(command.Role)))
                        return Result.Failure(TransferTicketError.ApproverUnAuthorized());

                    selectTransferRequestId.IsApprove = true;

                    var userApprovalId = await _context.ApproverTicketings
                        .Where(x => x.TransferTicketConcernId == selectTransferRequestId.TransferTicketConcernId)
                        .ToListAsync();

                    var validateUserApprover = userApprovalId
                        .FirstOrDefault(x => x.ApproverLevel == selectTransferRequestId.ApproverLevel + 1);

                    await ApprovalTicketHistory(ticketHistoryList, userDetails, command, cancellationToken);

                    if(validateUserApprover is not null)
                    {
                        await ApprovalTransferNotification(transferTicketExist,userDetails,validateUserApprover,command,cancellationToken);
                    }
                    else
                    {
                        await UpdateTransferTicket(transferTicketExist, userDetails, command, cancellationToken);
                    }
                }
                else
                {
                    return Result.Failure(TransferTicketError.ApproverUnAuthorized());
                }

                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }

            private async Task ApprovalTicketHistory(List<TicketHistory> ticketHistory, User user, ApprovedTransferTicketCommand command, CancellationToken cancellationToken)
            {
                var ticketHistoryApproval = ticketHistory
                    .FirstOrDefault(x => x.Approver_Level != null
                    && x.Approver_Level == ticketHistory.Min(x => x.Approver_Level));

                ticketHistoryApproval.TransactedBy = command.Transacted_By;
                ticketHistoryApproval.TransactionDate = DateTime.Now;
                ticketHistoryApproval.Request = TicketingConString.Approve;
                ticketHistoryApproval.Status = $"{TicketingConString.TransferApprove} {user.Fullname}";
                ticketHistoryApproval.IsApprove = true;
            }

            private async Task UpdateTransferTicket(TransferTicketConcern transferTicketConcern,User user ,ApprovedTransferTicketCommand command, CancellationToken cancellationToken)
            {
                transferTicketConcern.TicketApprover = null;
                transferTicketConcern.IsTransfer = true;
                transferTicketConcern.TransferBy = transferTicketConcern.TicketConcern.UserId;
                transferTicketConcern.TransferAt = DateTime.Now;

                var ticketConcernExist = await _context.TicketConcerns
                    .Include(x => x.RequestorByUser)
                    .FirstOrDefaultAsync(x => x.Id == transferTicketConcern.TicketConcernId);

                ticketConcernExist.IsTransfer = null;
                ticketConcernExist.Remarks = transferTicketConcern.TransferRemarks;
                ticketConcernExist.UserId = transferTicketConcern.TransferTo;
                ticketConcernExist.TargetDate = transferTicketConcern.TargetDate;

                await ApprovedTicketNotification(transferTicketConcern, user, command, cancellationToken);

            }

            private async Task ApprovedTicketNotification(TransferTicketConcern transferTicketConcern,User user, ApprovedTransferTicketCommand command, CancellationToken cancellationToken)
            {

                var addTransferByTransactionNotification = new TicketTransactionNotification
                {

                    Message = $"Ticket concern number {transferTicketConcern.TicketConcernId} has transfer",
                    AddedBy = user.Id,
                    Created_At = DateTime.Now,
                    ReceiveBy = transferTicketConcern.AddedBy.Value,
                    Modules = PathConString.IssueHandlerConcerns,
                    Modules_Parameter = PathConString.ForTransfer,
                    PathId = transferTicketConcern.TicketConcernId ,

                };

                await _context.TicketTransactionNotifications.AddAsync(addTransferByTransactionNotification);

                var addTransferToTransactionNotification = new TicketTransactionNotification
                {

                    Message = $"Ticket concern number {transferTicketConcern.TicketConcernId} has transfer",
                    AddedBy = user.Id,
                    Created_At = DateTime.Now,
                    ReceiveBy = transferTicketConcern.TransferTo.Value,
                    Modules = PathConString.IssueHandlerConcerns,
                    Modules_Parameter = PathConString.OpenTicket,
                    PathId = transferTicketConcern.TicketConcernId,

                };

                await _context.TicketTransactionNotifications.AddAsync(addTransferToTransactionNotification);

            }

            private async Task ApprovalTransferNotification(TransferTicketConcern transferTicketConcern, User user, ApproverTicketing approverTicketing, ApprovedTransferTicketCommand command, CancellationToken cancellationToken)
            {
                transferTicketConcern.TicketApprover = approverTicketing.UserId;

                if(transferTicketConcern.TargetDate != command.Target_Date)
                    transferTicketConcern.TargetDate = command.Target_Date;

                var addNewTicketTransactionNotification = new TicketTransactionNotification
                {

                    Message = $"Ticket number {transferTicketConcern.TicketConcernId} was approved by {user.Fullname}",
                    AddedBy = user.Id,
                    Created_At = DateTime.Now,
                    ReceiveBy = approverTicketing.UserId.Value,
                    Modules = PathConString.Approval,
                    Modules_Parameter = PathConString.ForTransfer,
                    PathId = transferTicketConcern.TicketConcernId

                };

                await _context.TicketTransactionNotifications.AddAsync(addNewTicketTransactionNotification);

                var addTicketApproveNotification = new TicketTransactionNotification
                {

                    Message = $"Ticket number {transferTicketConcern.TicketConcernId} was approved by {user.Fullname}",
                    AddedBy = user.Id,
                    Created_At = DateTime.Now,
                    ReceiveBy = transferTicketConcern.TransferBy.Value,
                    Modules = PathConString.IssueHandlerConcerns,
                    Modules_Parameter = PathConString.ForTransfer,
                    PathId = transferTicketConcern.TicketConcernId

                };

                await _context.TicketTransactionNotifications.AddAsync(addTicketApproveNotification);

            }
        }
    }
}
