
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.BackJob
{
    public partial class TicketBackJob
    {

        public class Handler : IRequestHandler<TicketBackJobQuery, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(TicketBackJobQuery request, CancellationToken cancellationToken)
            {

                var result = await _context.TicketConcerns
                    .AsNoTrackingWithIdentityResolution()
                    .Include(r => r.RequestConcern)
                    .AsSplitQuery()
                    .Where(r => r.UserId == request.UserId)
                    .Where(r => r.IsClosedApprove == true && r.RequestConcern.Is_Confirm == true)
                    .Select(r => new TicketBackJobResult
                    {
                        TicketConcernId = r.Id,
                        Concern = r.RequestConcern.Concern,

                    }).ToListAsync();


                if(!string.IsNullOrEmpty(request.Search))
                {
                    result = result
                        .Where(r => r.TicketConcernId.ToString().Contains(request.Search))
                        .ToList();

                }


                return Result.Success(result);
            }
        }
    }
}
