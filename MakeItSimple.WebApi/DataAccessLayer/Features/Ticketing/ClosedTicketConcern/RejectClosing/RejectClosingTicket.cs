using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosedTicketConcern.RejectClosing
{
    public partial class RejectClosingTicket
    {

        public class Handler : IRequestHandler<RejectClosingTicketCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(RejectClosingTicketCommand command, CancellationToken cancellationToken)
            {
                var userDetails = await _context.Users
                    .FirstOrDefaultAsync(x => x.Id == command.Transacted_By);

                var closedTicketExist = await _context.ClosingTickets
                    .Include(c => c.TicketConcern)
                        .FirstOrDefaultAsync(x => x.Id == command.ClosingTicketId);

                if (closedTicketExist is null)          
                    return Result.Failure(ClosingTicketError.ClosingTicketIdNotExist());

                await UpdateCloseStatus(closedTicketExist,command,cancellationToken);
                await ClosingTicketHistory(userDetails,closedTicketExist,command,cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }


            private async Task UpdateCloseStatus(ClosingTicket closingTicket ,RejectClosingTicketCommand command, CancellationToken cancellationToken)
            {

                closingTicket.RejectClosedAt = DateTime.Now;
                closingTicket.IsRejectClosed = true;
                closingTicket.RejectClosedBy = command.RejectClosed_By;
                closingTicket.RejectRemarks = command.Reject_Remarks;

                var ticketConcernExist = await _context.TicketConcerns
                    .FirstOrDefaultAsync(x => x.Id == closingTicket.TicketConcernId);

                ticketConcernExist.IsClosedApprove = null;
                ticketConcernExist.Remarks = command.Reject_Remarks;

                var approverList = await _context.ApproverTicketings
                    .Where(x => x.ClosingTicketId == command.ClosingTicketId)
                    .ToListAsync();

                foreach (var transferTicket in approverList)
                {
                    _context.Remove(transferTicket);
                }

                var ticketHistory = await _context.TicketHistories
                    .Where(x => x.TicketConcernId == closingTicket.TicketConcernId)
                    .Where(x => x.IsApprove == null && x.Request.Contains(TicketingConString.Approval)
                     || x.Request.Contains(TicketingConString.NotConfirm))
                    .ToListAsync();

                foreach (var item in ticketHistory)
                {
                    _context.TicketHistories.Remove(item);
                }

            }

            private async Task ClosingTicketHistory(User user, ClosingTicket closingTicket, RejectClosingTicketCommand command, CancellationToken cancellationToken)
            {
                var addTicketHistory = new TicketHistory
                {
                    TicketConcernId = closingTicket.TicketConcernId,
                    TransactedBy = command.Transacted_By,
                    TransactionDate = DateTime.Now,
                    Request = TicketingConString.Reject,
                    Status = $"{TicketingConString.CloseReject} {user.Fullname}",
                    Remarks = command.Reject_Remarks,

                };

                await _context.TicketHistories.AddAsync(addTicketHistory, cancellationToken);

                var addNewTicketTransactionNotification = new TicketTransactionNotification
                {

                    Message = $"Closing request for ticket number {closingTicket.TicketConcernId} was rejected.",
                    AddedBy = user.Id,
                    Created_At = DateTime.Now,
                    ReceiveBy = closingTicket.TicketConcern.UserId.Value,
                    Modules = PathConString.IssueHandlerConcerns,
                    Modules_Parameter = PathConString.OpenTicket,
                    PathId = closingTicket.TicketConcernId,

                };

                await _context.TicketTransactionNotifications.AddAsync(addNewTicketTransactionNotification);
            }


        }
    }
}
