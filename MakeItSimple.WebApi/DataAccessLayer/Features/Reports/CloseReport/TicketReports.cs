using Humanizer;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Reports.CloseReport
{
    public partial class TicketReports
    {


        public class Handler : IRequestHandler<TicketReportsQuery, PagedList<Reports>>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<Reports>> Handle(TicketReportsQuery request, CancellationToken cancellationToken)
            {

                IQueryable<TicketConcern> ticketQuery = _context.TicketConcerns
                    .AsNoTrackingWithIdentityResolution()
                    .Include(x => x.AddedByUser)
                    .Include(x => x.ModifiedByUser)
                    .Include(x => x.RequestorByUser)
                    .Include(x => x.User)
                    .ThenInclude(x => x.SubUnit)
                    .Include(x => x.ClosingTickets)
                    .ThenInclude(x => x.TicketAttachments)
                    .Include(x => x.TransferTicketConcerns)
                    .ThenInclude(x => x.TicketAttachments)
                    .Include(x => x.RequestConcern)
                    .Include(x => x.RequestConcern)
                    .ThenInclude(x => x.Channel)
                    .AsSplitQuery();
                    


                if (request.Unit is not null)
                {
                    ticketQuery = ticketQuery.Where(x => x.User.UnitId == request.Unit);

                    if (request.UserId is not null)
                    {
                        ticketQuery = ticketQuery.Where(x => x.UserId == request.UserId);
                    }
                }

                if (!string.IsNullOrEmpty(request.Remarks))
                {
                    switch (request.Remarks)
                    {
                        case TicketingConString.OnTime:
                            ticketQuery = ticketQuery
                                .Where(x => x.Closed_At != null && x.TargetDate.Value > x.Closed_At.Value);
                            break;

                        case TicketingConString.Delay:
                            ticketQuery = ticketQuery
                                .Where(x => x.Closed_At != null && x.TargetDate.Value < x.Closed_At.Value);
                            break;

                        default:
                            return new PagedList<Reports>(new List<Reports>(), 0, request.PageNumber, request.PageSize);

                    }
                }

                if (!string.IsNullOrEmpty(request.Search))
                {
                    ticketQuery = ticketQuery
                        .Where(x => x.Id.ToString().Contains(request.Search)
                        || x.User.Fullname.Contains(request.Search));
                }

                ticketQuery = ticketQuery
                    .Where(x => x.TargetDate.HasValue && x.TargetDate.Value.Date >= request.Date_From.Value.Date && x.TargetDate.Value.Date <= request.Date_To.Value.Date);


                var results = ticketQuery
                    .Where(x => x.RequestConcern.Is_Confirm == true && x.IsClosedApprove == true)

                    .Select(x => new Reports
                    {
                        Year = x.TargetDate.Value.Date.Year.ToString(),
                        Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(x.TargetDate.Value.Date.Month),
                        Start_Date = $"{x.TargetDate.Value.Date.Month}-01-{x.TargetDate.Value.Date.Year}",
                        End_Date = $"{x.TargetDate.Value.Date.Month}-{DateTime.DaysInMonth(x.TargetDate.Value.Date.Year, x.TargetDate.Value.Date.Month)}-{x.TargetDate.Value.Date.Year}",
                        Personnel = x.User.Fullname,
                        Ticket_Number = x.Id,
                        Description = x.RequestConcern.Concern,
                        Target_Date = $"{x.TargetDate.Value.Date.Month}-{x.TargetDate.Value.Date.Day}-{x.TargetDate.Value.Date.Year}",
                        Actual =  $"{x.Closed_At.Value.Date.Month}-{x.Closed_At.Value.Date.Day}-{x.Closed_At.Value.Date.Year}",
                        Varience = EF.Functions.DateDiffDay(x.TargetDate.Value.Date, x.Closed_At.Value.Date),
                        Efficeincy = x.TargetDate > x.Closed_At ? $"100 %" : "50 %",
                        Status = TicketingConString.Closed,
                        Remarks =  x.TargetDate.Value > x.Closed_At.Value ? TicketingConString.OnTime : TicketingConString.Delay,
                        Category = x.RequestConcern.Channel.ChannelName,
                    });

                return await PagedList<Reports>.CreateAsync(results, request.PageNumber, request.PageSize);
            }
        }

    }
}
