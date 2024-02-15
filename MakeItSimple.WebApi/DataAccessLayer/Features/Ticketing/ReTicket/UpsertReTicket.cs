﻿using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.Xml;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ReTicket
{
    public class UpsertReTicket
    {
        public class UpsertReTicketCommand : IRequest<Result>
        {
            public Guid? Added_By { get; set; }
            public Guid? Modified_By { get; set; }
            public int ? RequestGeneratorId { get; set; }
            public string Re_Ticket_Remarks { get; set; }
            public List<UpsertReTicketConsern> UpsertReTicketConserns {  get; set; }

            public class UpsertReTicketConsern
            {
                public int ? TicketConcernId { get; set; }
                public int ? ReTicketConcernId { get; set; }
                public DateTime? Start_Date { get; set; }
                public DateTime? Target_Date { get; set; }

            }

        }

        public class Handler : IRequestHandler<UpsertReTicketCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(UpsertReTicketCommand command, CancellationToken cancellationToken)
            {

                var requestGeneratorIdInTransfer = await _context.ReTicketConcerns.FirstOrDefaultAsync(x => x.RequestGeneratorId == command.RequestGeneratorId, cancellationToken);

                if (requestGeneratorIdInTransfer == null)
                {
                    return Result.Failure(ReTicketConcernError.TicketIdNotExist());
                }

                var validateApprover = await _context.ApproverTicketings.FirstOrDefaultAsync(x => x.RequestGeneratorId == requestGeneratorIdInTransfer.RequestGeneratorId
                && x.IsApprove != null , cancellationToken);

                if(validateApprover is not null)
                {
                    return Result.Failure(ReTicketConcernError.ReTicketConcernUnable());
                }
                //else if(command.UpsertReTicketConserns.Count == 0)
                //{
                //  return 
                //}

                foreach (var reTicket in command.UpsertReTicketConserns)
                {
                    var ticketConcern = await _context.TicketConcerns.FirstOrDefaultAsync(x => x.Id == reTicket.TicketConcernId, cancellationToken);
                    if (ticketConcern == null)
                    {

                        return Result.Failure(TransferTicketError.TicketConcernIdNotExist());
                    }

                    if (command.UpsertReTicketConserns.Count(x => x.TicketConcernId == reTicket.TicketConcernId) > 1)
                    {
                        return Result.Failure(TransferTicketError.DuplicateConcernTicket());
                    }
                    else if (command.UpsertReTicketConserns.Count(x => x.ReTicketConcernId == reTicket.ReTicketConcernId) > 1)
                    {
                        return Result.Failure(TransferTicketError.DuplicateTransferTicket());
                    }

                    var reTicketConcern = await _context.ReTicketConcerns.FirstOrDefaultAsync(x => x.Id == reTicket.ReTicketConcernId, cancellationToken);
                    if (ticketConcern != null && reTicketConcern != null)
                    {

                        var ticketConcernAlreadyExist = await _context.ReTicketConcerns.FirstOrDefaultAsync(x => x.TicketConcernId == reTicket.TicketConcernId 
                        && reTicket.TicketConcernId != reTicket.TicketConcernId && x.IsReTicket == false, cancellationToken);
                        if (ticketConcernAlreadyExist != null)
                        {
                            return Result.Failure(TransferTicketError.TransferTicketAlreadyExist());
                        }

                        bool HasChange = false;

                        if (reTicketConcern.ReTicketRemarks != command.Re_Ticket_Remarks)
                        {
                            reTicketConcern.ReTicketRemarks = command.Re_Ticket_Remarks;
                            HasChange = true;
                        }

                        if (reTicketConcern.StartDate != reTicket.Start_Date)
                        {
                            reTicketConcern.StartDate = reTicket.Start_Date;
                            HasChange = true;
                        }

                        if (reTicketConcern.TargetDate != reTicket.Target_Date)
                        {
                            reTicketConcern.TargetDate = reTicket.Target_Date;
                            HasChange = true;
                        }

                        if (HasChange is true)
                        {
                            reTicketConcern.ModifiedBy = command.Modified_By;
                            reTicketConcern.UpdatedAt = DateTime.Now;
                            reTicketConcern.IsRejectReTicket = false;
                            reTicketConcern.IsReTicket = false;
                            reTicketConcern.ReTicketAt = null;
                            reTicketConcern.ReTicketBy = null;

                        }

                    }
                    else if (ticketConcern != null && reTicketConcern is null)
                    {
                        var reTicketAlreadyExist = await _context.ReTicketConcerns.FirstOrDefaultAsync(x => x.TicketConcernId == reTicketConcern.TicketConcernId 
                        && x.IsReTicket == false, cancellationToken);

                        if (reTicketAlreadyExist != null)
                        {
                            return Result.Failure(ReTicketConcernError.TicketConcernIdAlreadyExist());
                        }

                        var addReTicket = new ReTicketConcern
                        {
                            RequestGeneratorId = requestGeneratorIdInTransfer.RequestGeneratorId,

                            ConcernDetails = ticketConcern.ConcernDetails,
                            CategoryId = ticketConcern.CategoryId,
                            SubCategoryId = ticketConcern.SubCategoryId,
                            ReTicketRemarks = command.Re_Ticket_Remarks,
                            AddedBy = command.Added_By,
                            StartDate = reTicket.Start_Date,
                            TargetDate = reTicket.Target_Date,
                            IsReTicket = false,
                            IsRejectReTicket = false,
                            RejectTransferAt = null,
                            RejectReTicketBy = null,
                            TicketApprover = requestGeneratorIdInTransfer.TicketApprover

                        };



                        await _context.ReTicketConcerns.AddAsync(addReTicket, cancellationToken);
                    }
                    else
                    {
                        return Result.Failure(TransferTicketError.TicketIdNotExist());
                    }

                    ticketConcern.IsReTicket = false;
                }

                await _context.SaveChangesAsync();
                return Result.Success();
            }
        }
    }
}
