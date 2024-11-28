using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosedTicketConcern.Get_Ticket_Technician
{
    public class GetTicketTechnician
    {
        public class GetTicketTechnicianResult
        {
            public Guid? TechnicianId { get; set; }
            public string Technician_Name { get; set; }
        }

        public class GetTicketTechnicianQuery : IRequest<Result>
        {
            public int TicketConcernId { get; set; }
        }

        public class Handler : IRequestHandler<GetTicketTechnicianQuery, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(GetTicketTechnicianQuery request, CancellationToken cancellationToken)
            {
                var ticketTechnician = _context.TicketTechnicians
                    .Where(x => x.ClosingTicket.TicketConcernId == request.TicketConcernId)
                    .Select(x => x.TechnicianBy);

                var query = await _context.Users
                    .Where(x => x.IsActive &&
                    x.UserRole.UserRoleName.Contains(TicketingConString.Technician)
                    && !ticketTechnician.Contains(x.Id))
                    .Select(x => new GetTicketTechnicianResult
                    {
                        TechnicianId = x.Id,
                        Technician_Name = x.Fullname,

                    }).ToListAsync();

                return Result.Success(query);
            }
        }
    }
}
