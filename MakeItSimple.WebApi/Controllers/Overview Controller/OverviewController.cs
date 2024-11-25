using MakeItSimple.WebApi.Common.Extension;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Features.Overview.Ticket_Overview;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Overview.Ticket_Overview.TicketOverview;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Reports.AllTicketReport.AllTicketReports;
using System.Security.Claims;

namespace MakeItSimple.WebApi.Controllers.Overview_Controller
{
    [Route("api/overview")]
    [ApiController]
    public class OverviewController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OverviewController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpGet("ticket")]
        public async Task<IActionResult> TicketOverview([FromQuery] TicketOverviewQuery query)
        {
            try
            {
                if (User.Identity is ClaimsIdentity identity)
                {
                    var userRole = identity.FindFirst(ClaimTypes.Role);
                    if (userRole != null)
                    {
                        query.Role = userRole.Value;
                    }

                    if (Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                    {
                        query.UserId = userId;
                    }

                }

                var overview = await _mediator.Send(query);

                Response.AddPaginationHeader(

                overview.CurrentPage,
                overview.PageSize,
                overview.TotalCount,
                overview.TotalPages,
                overview.HasPreviousPage,
                overview.HasNextPage

                );

                var result = new
                {
                    overview,
                    overview.CurrentPage,
                    overview.PageSize,
                    overview.TotalCount,
                    overview.TotalPages,
                    overview.HasPreviousPage,
                    overview.HasNextPage
                };

                var successResult = Result.Success(result);

                return Ok(successResult);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }

        }


    }
}
