﻿using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.OnHoldTicket.CreateOnHold.CreateOnHoldTicket;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.OnHoldTicket.ResumeOnHold
{
    public partial class ResumeOnHoldTicket
    {
        public class Handler : IRequestHandler<ResumeOnHoldTicketCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(ResumeOnHoldTicketCommand command, CancellationToken cancellationToken)
            {

                var onHoldExist = await _context.TicketOnHolds
                    .FirstOrDefaultAsync(o => o.Id == command.TicketOnHoldId, cancellationToken);

                if (onHoldExist is null)
                    return Result.Failure(TicketRequestError.TicketConcernIdNotExist());

                await UpdateOnHold(onHoldExist,command,cancellationToken);
                await OnHoldTicketHistory(onHoldExist, command, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }


            private async Task<TicketOnHold> UpdateOnHold(TicketOnHold ticketOnHold, ResumeOnHoldTicketCommand command, CancellationToken cancellationToken)
            {
                ticketOnHold.IsHold = null;
                ticketOnHold.ResumeAt = DateTime.Now;

                var ticketConcern = await _context.TicketConcerns
                    .FirstOrDefaultAsync(t => t.Id == ticketOnHold.TicketConcernId, cancellationToken);

                ticketConcern.OnHold = null;
                ticketConcern.Resume_At = DateTime.Now;

                return ticketOnHold;

            }

            private async Task<TicketHistory> OnHoldTicketHistory(TicketOnHold onHold, ResumeOnHoldTicketCommand command, CancellationToken cancellationToken)
            {
                var addTicketHistory = new TicketHistory
                {
                    TicketConcernId = onHold.TicketConcernId,
                    TransactedBy = command.Transacted_By,
                    TransactionDate = DateTime.Now,
                    Request = TicketingConString.OnHold,
                    Status = TicketingConString.Resume

                };

                await _context.TicketHistories.AddAsync(addTicketHistory);

                return addTicketHistory;
            }


        }
    }
}
