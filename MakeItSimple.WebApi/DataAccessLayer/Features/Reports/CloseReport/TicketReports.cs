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
                    .AsSplitQuery()
                    .Where(x => x.IsClosedApprove == true && x.ClosingTickets.FirstOrDefault(x => x.IsClosing == true).IsActive == true)
                    .Where(x => x.Closed_At.Value.Date >= request.Date_From.Value.Date && x.Closed_At.Value.Date <= request.Date_To.Value.Date);

                var closingTicketTechnician = _context.TicketTechnicians
                      .AsNoTrackingWithIdentityResolution()
                      .Include(x => x.TechnicianByUser)
                      .Include(x => x.ClosingTicket)
                      .ThenInclude(x => x.TicketConcern)
                      .AsSplitQuery()
                      .Where(x => x.ClosingTicket.TicketConcern.IsClosedApprove == true && x.ClosingTicket.IsActive == true && x.ClosingTicket.IsClosing == true)
                      .Where(x => x.ClosingTicket.TicketConcern.Closed_At.Value.Date >= request.Date_From.Value.Date && x.ClosingTicket.TicketConcern.Closed_At.Value.Date <= request.Date_To.Value.Date)
                      .Select(x => new
                      {
                          x.ClosingTicket.TicketConcern.TargetDate.Value.Year,
                          x.ClosingTicket.TicketConcern.TargetDate.Value.Month,
                          x.ClosingTicket.TicketConcern.TargetDate,
                          ClosedAt = x.ClosingTicket.TicketConcern.Closed_At,
                          TechnicianName = x.TechnicianByUser.Fullname,
                          TicketId = x.ClosingTicket.TicketConcernId,
                          ConcernDescription = x.ClosingTicket.TicketConcern.RequestConcern.Concern,
                          x.ClosingTicket.TicketConcern.RequestConcern.Channel.ChannelName
                      });

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

                var closingTicket = ticketQuery
                        .Select(x => new
                        {
                            x.TargetDate.Value.Year,
                            x.TargetDate.Value.Month,
                            x.TargetDate,
                            ClosedAt = x.Closed_At,
                            TechnicianName = x.User.Fullname,
                            TicketId = x.Id,
                            ConcernDescription = x.RequestConcern.Concern,
                            x.RequestConcern.Channel.ChannelName
                        });

                var combinedTickets = closingTicket
                    .Concat(closingTicketTechnician);

                var results = combinedTickets.Select(x => new Reports
                {
                    Year = x.Year.ToString(),
                    Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(x.Month),
                    Start_Date = $"{x.Month}-01-{x.Year}",
                    End_Date = $"{x.Month}-{DateTime.DaysInMonth(x.Year, x.Month)}-{x.Year}",
                    Personnel = x.TechnicianName,
                    Ticket_Number = x.TicketId,
                    Description = x.ConcernDescription,
                    Target_Date = x.TargetDate.Value.ToString("MM-dd-yyyy"),
                    Actual = x.ClosedAt.HasValue ? x.ClosedAt.Value.ToString("MM-dd-yyyy") : "N/A",
                    Varience = EF.Functions.DateDiffDay(x.TargetDate.Value, x.ClosedAt.Value),
                    Efficeincy = x.ClosedAt.Value.Date <= x.TargetDate.Value.Date ? "100 %" : "50 %",
                    Status = TicketingConString.Closed,
                    Remarks = x.ClosedAt.Value.Date <= x.TargetDate.Value.Date ? TicketingConString.OnTime : TicketingConString.Delay,
                    Category = x.ChannelName

                }).OrderBy(x => x.Ticket_Number); 

                return await PagedList<Reports>.CreateAsync(results, request.PageNumber, request.PageSize);
            }
        }

    }
}
