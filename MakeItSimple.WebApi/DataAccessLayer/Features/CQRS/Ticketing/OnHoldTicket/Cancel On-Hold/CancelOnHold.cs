using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket.CancelTransferTicket;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.OnHoldTicket.Cancel_On_Hold
{
    public partial class CancelOnHold
    {
        public class Handler : IRequestHandler<CancelOnHoldCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(CancelOnHoldCommand command, CancellationToken cancellationToken)
            {
                var onHoldTicketExist = await _context.TicketOnHolds
                .FirstOrDefaultAsync(x => x.Id == command.OnHoldTicketId);

                if (onHoldTicketExist is null)
                    return Result.Failure(TicketOnHoldError.TicketOnHoldIdNotExist());

                if (onHoldTicketExist.IsHold is true)
                    return Result.Failure(TicketRequestError.TicketAlreadyApproved());

                if (onHoldTicketExist.IsRejectOnHold is true)
                    return Result.Failure(TransferTicketError.TransferAlreadyReject());

                onHoldTicketExist.IsActive = false;

                var ticketConcernExist = await _context.TicketConcerns
                    .FirstOrDefaultAsync(x => x.Id == onHoldTicketExist.TicketConcernId);

                ticketConcernExist.OnHold = null;

                var approverList = await _context.ApproverTicketings
                    .Where(x => x.TicketOnHoldId == command.OnHoldTicketId)
                    .ToListAsync();

                foreach (var ticketOnHold in approverList)
                {
                    _context.ApproverTicketings.Remove(ticketOnHold);
                }

                var ticketHistory = await _context.TicketHistories
                    .Where(x => (x.TicketConcernId == ticketConcernExist.Id
                     && x.IsApprove == null && x.Request.Contains(TicketingConString.Approval)))
                    .ToListAsync();

                foreach (var item in ticketHistory)
                {
                    _context.TicketHistories.Remove(item);
                }

                await CreateTicketHistory(ticketConcernExist, command, cancellationToken);

                await _context.SaveChangesAsync();
                return Result.Success();
            }


            private async Task CreateTicketHistory(TicketConcern ticketConcern, CancelOnHoldCommand command, CancellationToken cancellationToken)
            {
                var addTicketHistory = new TicketHistory
                {
                    TicketConcernId = ticketConcern.Id,
                    TransactedBy = command.Transacted_By,
                    TransactionDate = DateTime.Now,
                    Request = TicketingConString.Cancel,
                    Status = TicketingConString.OnHoldCancel,
                };

                await _context.TicketHistories.AddAsync(addTicketHistory, cancellationToken);

            }

            private async Task CreateTransactionNotification(TicketConcern ticketConcern, TicketOnHold ticketOnHold, CancelTransferTicketCommand command, CancellationToken cancellationToken)
            {
                var addNewTicketTransactionNotification = new TicketTransactionNotification
                {

                    Message = $"Ticket on-hold request #{ticketConcern.Id} has been canceled",
                    AddedBy = ticketOnHold.AddedBy.Value,
                    Created_At = DateTime.Now,
                    ReceiveBy = ticketOnHold.TicketApprover.Value,
                    Modules = PathConString.IssueHandlerConcerns,
                    Modules_Parameter = PathConString.OpenTicket,
                    PathId = ticketConcern.Id

                };

                await _context.TicketTransactionNotifications.AddAsync(addNewTicketTransactionNotification);
            }


        }
    }
}
