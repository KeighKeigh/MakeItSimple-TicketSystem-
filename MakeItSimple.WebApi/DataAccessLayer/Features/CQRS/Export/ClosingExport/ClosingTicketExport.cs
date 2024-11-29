using ClosedXML.Excel;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Export.ClosingExport
{
    public partial class ClosingTicketExport
    {

        public class Handler : IRequestHandler<ClosingTicketExportCommand, Unit>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Unit> Handle(ClosingTicketExportCommand request, CancellationToken cancellationToken)
            {

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
                           Unit = x.TechnicianByUser.UnitId,
                           UserId =  x.TechnicianBy,
                            x.ClosingTicket.TicketConcern.TargetDate.Value.Year,
                            x.ClosingTicket.TicketConcern.TargetDate.Value.Month,
                            x.ClosingTicket.TicketConcern.TargetDate,
                            ClosedAt = x.ClosingTicket.TicketConcern.Closed_At,
                            TechnicianName = x.TechnicianByUser.Fullname,
                            TicketId = x.ClosingTicket.TicketConcernId,
                            ConcernDescription = x.ClosingTicket.TicketConcern.RequestConcern.Concern,
                            x.ClosingTicket.TicketConcern.RequestConcern.Channel.ChannelName
                        });

                var closingTicket =  _context.TicketConcerns
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
                    .ThenInclude(x => x.Channel)
                    .AsSplitQuery()
                    .Where(x => x.IsClosedApprove == true && x.ClosingTickets.FirstOrDefault(x => x.IsClosing == true).IsActive == true)
                    .Where(x => x.Closed_At.Value.Date >= request.Date_From.Value.Date && x.Closed_At.Value.Date <= request.Date_To.Value.Date)
                    .Select(x => new
                    {
                        Unit = x.User.UnitId,
                        x.UserId,
                        x.TargetDate.Value.Year,
                        x.TargetDate.Value.Month,
                        x.TargetDate,
                        ClosedAt = x.Closed_At,
                        TechnicianName = x.User.Fullname,
                        TicketId = x.Id,
                        ConcernDescription = x.RequestConcern.Concern,
                        x.RequestConcern.Channel.ChannelName
                    });

                var combineTicket = closingTicket
                    .Concat(closingTicketTechnician);

                var closing = await combineTicket
                    .Select(x => new ClosingTicketExportResult
                    {
                        Unit = x.Unit,
                        UserId = x.UserId,
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

                    }).ToListAsync();


                if (request.Unit is not null)
                {
                    closing = closing.Where(x => x.Unit == request.Unit).ToList();

                    if (request.UserId is not null)
                    {
                        closing = closing.Where(x => x.UserId == request.UserId).ToList();
                    }
                }

                if (!string.IsNullOrEmpty(request.Remarks))
                {
                    switch (request.Remarks)
                    {
                        case TicketingConString.OnTime:
                            closing = closing
                                .Where(x => x.Actual != null && x.Target_Date_DateTime > x.Actual_Date_DateTime)
                                .ToList();
                            break;

                        case TicketingConString.Delay:
                            closing = closing
                                .Where(x => x.Actual != null && x.Target_Date_DateTime < x.Actual_Date_DateTime)
                                .ToList();
                            break;

                        default:
                            return Unit.Value;

                    }
                }

                if (!string.IsNullOrEmpty(request.Search))
                {
                    closing = closing
                        .Where(x => x.Ticket_Number.ToString().Contains(request.Search)
                        || x.Personnel.Contains(request.Search))
                        .ToList();
                }


                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add($"Closing Ticket Report");
                    var headers = new List<string>
                    {
                        "Year",
                        "Month",
                        "Start Date",
                        "End Date",
                        "Personnel",
                        "Ticket Number",
                        "Description",
                        "Target Date",
                        "Actual",
                        "Varience",
                        "Efficiency",
                        "Remarks",
                        "Category"

                    };

                    var range = worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, headers.Count));

                    range.Style.Fill.BackgroundColor = XLColor.LavenderPurple;
                    range.Style.Font.Bold = true;
                    range.Style.Font.FontColor = XLColor.Black;
                    range.Style.Border.TopBorder = XLBorderStyleValues.Thick;
                    range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    for (var index = 1; index <= headers.Count; index++)
                    {
                        worksheet.Cell(1, index).Value = headers[index - 1];
                    }
                    for (var index = 1; index <= closing.Count; index++)
                    {
                        var row = worksheet.Row(index + 1);

                        row.Cell(1).Value = closing[index - 1].Year;
                        row.Cell(2).Value = closing[index - 1].Month;
                        row.Cell(3).Value = closing[index - 1].Start_Date;
                        row.Cell(4).Value = closing[index - 1].End_Date;
                        row.Cell(5).Value = closing[index - 1].Personnel;
                        row.Cell(6).Value = closing[index - 1].Ticket_Number;
                        row.Cell(7).Value = closing[index - 1].Description;
                        row.Cell(8).Value = closing[index - 1].Target_Date;
                        row.Cell(9).Value = closing[index - 1].Actual;
                        row.Cell(10).Value = closing[index - 1].Varience;
                        row.Cell(11).Value = closing[index - 1].Efficeincy;
                        row.Cell(12).Value = closing[index - 1].Remarks;
                        row.Cell(13).Value = closing[index - 1].Category;

                    }

                    worksheet.Columns().AdjustToContents();
                    workbook.SaveAs($"ClosingTicketReports {request.Date_From:MM-dd-yyyy} - {request.Date_To:MM-dd-yyyy}.xlsx");

                }

                return Unit.Value;
            }
        }
    }
}
