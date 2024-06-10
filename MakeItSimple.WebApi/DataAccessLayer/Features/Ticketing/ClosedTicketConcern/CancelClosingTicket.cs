﻿using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosedTicketConcern
{
    public class CancelClosingTicket
    {
        public class CancelClosingTicketCommand : IRequest<Result>
        {
            public Guid ? Requestor_By {  get; set; }
            public int? ClosingTicketId { get; set; }
        }

        public class Handler : IRequestHandler<CancelClosingTicketCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(CancelClosingTicketCommand command, CancellationToken cancellationToken)
            {

                var closingTicketExist = await _context.ClosingTickets
                    .Where(x => x.Id == command.ClosingTicketId)
                    .FirstOrDefaultAsync();

                if (closingTicketExist == null)
                {
                    return Result.Failure(ClosingTicketError.ClosingTicketIdNotExist());
                }

                closingTicketExist.IsActive = false;

                var ticketConcernId = await _context.TicketConcerns
                    .FirstOrDefaultAsync(x => x.Id == closingTicketExist.TicketConcernId);

                ticketConcernId.IsClosedApprove = null;

                var approverList = await _context.ApproverTicketings
                    .Where(x => x.ClosingTicketId == command.ClosingTicketId)
                    .ToListAsync();

                foreach (var transferTicket in approverList)
                {
                    _context.Remove(transferTicket);
                }

                var addTicketHistory = new TicketHistory
                {
                    TicketConcernId = closingTicketExist.Id,
                    RequestorBy = command.Requestor_By,
                    TransactionDate = DateTime.Now,
                    Request = TicketingConString.CloseTicket,
                    Status = TicketingConString.Cancel
                };

                await _context.TicketHistories.AddAsync(addTicketHistory, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
        }
    }
}