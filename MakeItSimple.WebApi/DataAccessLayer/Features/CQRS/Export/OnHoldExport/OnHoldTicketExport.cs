using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using System.Runtime.InteropServices;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Reports.OnHoldReport.OnHoldTicketReport;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Export.OnHoldExport
{
    public partial class OnHoldTicketExport
    {

        public class Handler : IRequestHandler<OnHoldTicketExportQuery, Unit>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Unit> Handle(OnHoldTicketExportQuery request, CancellationToken cancellationToken)
            {

                var query = await _context.TicketOnHolds
                    .Include(q => q.TicketConcern)
                    .ThenInclude(q => q.RequestConcern)
                    .Include(q => q.AddedByUser)
                    .Where(r => r.CreatedAt.Date >= request.Date_From.Value && r.CreatedAt <= request.Date_To.Value)
                    .Select(r => new OnHoldTicketExportResult
                    {
                        UserId = r.AddedBy,
                        Unit = r.AddedByUser.UnitId,
                        TicketConcernId = r.TicketConcernId.Value,
                        Concerns = r.TicketConcern.RequestConcern.Concern,
                        Reason = r.Reason,
                        Added_By = r.AddedByUser.Fullname,
                        Created_At = r.CreatedAt,
                        IsHold = r.IsHold,
                        Resume_At = r.ResumeAt,
                    }).ToListAsync();


                if (request.Unit is not null)
                {
                    query = query.Where(x => x.Unit == request.Unit).ToList();

                    if (request.UserId is not null)
                    {
                        query = query.Where(x => x.UserId == request.UserId).ToList();
                    }
                }


                if (!string.IsNullOrEmpty(request.Search))
                {
                    query = query
                        .Where(x => x.TicketConcernId.ToString().Contains(request.Search)
                        || x.Added_By.Contains(request.Search)).ToList();
                }


                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add($"OnHold Ticket Report");
                    var headers = new List<string>
                    {
                        "Ticket Number",
                        "Description",
                        "Reason",
                        "OnHold By",
                        "OnHold At",
                        "Resume At",
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

                    for (var index = 1; index <= query.Count; index++)
                    {
                        var row = worksheet.Row(index + 1);

                        row.Cell(1).Value = query[index - 1].TicketConcernId;
                        row.Cell(2).Value = query[index - 1].Concerns;
                        row.Cell(3).Value = query[index - 1].Reason;
                        row.Cell(4).Value = query[index - 1].Added_By;
                        row.Cell(5).Value = query[index - 1].Created_At;
                        row.Cell(6).Value = query[index - 1].Resume_At;
                    }


                    worksheet.Columns().AdjustToContents();
                    workbook.SaveAs($"OnHoldTicketReports {request.Date_From:MM-dd-yyyy} - {request.Date_To:MM-dd-yyyy}.xlsx");

                }

                return Unit.Value;
            }
        }

    }
}
