﻿using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Reports.CloseReport.TicketReports;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

                var closing = await _context.TicketConcerns
                     .AsNoTracking()
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
                    .Where(x => x.TargetDate.Value.Date >= request.Date_From.Value.Date && x.TargetDate.Value.Date < request.Date_To.Value.Date)
                    .Where(x => x.IsClosedApprove == true && x.RequestConcern.Is_Confirm == true)
                    .Select(x => new ClosingTicketExportResult
                    {
                        Unit = x.User.UnitId,
                        UserId = x.UserId,
                        Year = x.TargetDate.Value.Date.Year.ToString(),
                        Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(x.TargetDate.Value.Date.Month),
                        Start_Date = $"{x.TargetDate.Value.Date.Month}-01-{x.TargetDate.Value.Date.Year}",
                        End_Date = $"{x.TargetDate.Value.Date.Month}-{DateTime.DaysInMonth(x.TargetDate.Value.Date.Year, x.TargetDate.Value.Date.Month)}-{x.TargetDate.Value.Date.Year}",
                        Personnel = x.User.Fullname,
                        Ticket_Number = x.Id,
                        Description = x.RequestConcern.Concern,
                        Target_Date_DateTime = x.TargetDate.Value.Date,
                        Actual_Date_DateTime = x.Closed_At.Value.Date,
                        Target_Date = $"{x.TargetDate.Value.Date.Month}-{x.TargetDate.Value.Date.Day}-{x.TargetDate.Value.Date.Year}",
                        Actual = $"{x.Closed_At.Value.Date.Month}-{x.Closed_At.Value.Date.Day}-{x.Closed_At.Value.Date.Year}",
                        Varience = EF.Functions.DateDiffDay(x.TargetDate.Value.Date, x.Closed_At.Value.Date),
                        Efficeincy = Math.Round(Math.Max(0, 100m - (decimal)EF.Functions.DateDiffDay(x.TargetDate.Value.Date, x.Closed_At.Value.Date)
                        / DateTime.DaysInMonth(x.TargetDate.Value.Date.Year, x.TargetDate.Value.Date.Month) * 100m), 2),
                        Status = TicketingConString.Closed,
                        Remarks = x.TargetDate.Value > x.Closed_At.Value ? TicketingConString.OnTime : TicketingConString.Delay,
                        Category = x.RequestConcern.Channel.ChannelName,
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
                        "Status",
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
                        row.Cell(12).Value = closing[index - 1].Status;
                        row.Cell(13).Value = closing[index - 1].Remarks;
                        row.Cell(14).Value = closing[index - 1].Category;


                    }

                    worksheet.Columns().AdjustToContents();
                    workbook.SaveAs($"ClosingTicketReports {request.Date_From:MM-dd-yyyy} - {request.Date_To:MM-dd-yyyy}.xlsx");

                }

                return Unit.Value;
            }
        }
    }
}
