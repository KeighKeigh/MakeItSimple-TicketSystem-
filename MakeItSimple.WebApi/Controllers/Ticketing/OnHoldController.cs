using MakeItSimple.WebApi.Common.Extension;
using MakeItSimple.WebApi.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.OnHoldTicket.Approval_On_Hold.ApprovalOnHold;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.OnHoldTicket.CreateOnHold.CreateOnHoldTicket;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.OnHoldTicket.ResumeOnHold.ResumeOnHoldTicket;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.OnHoldTicket.Get_OnHold.GetOnHold;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket.CancelTransferTicket;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.OnHoldTicket.Cancel_On_Hold.CancelOnHold;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket.RejectTransfer.RejectTransferTicket;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.OnHoldTicket.Reject_On_Hold.RejectOnHold;

namespace MakeItSimple.WebApi.Controllers.Ticketing
{
    [Route("api/on-hold")]
    [ApiController]
    public class OnHoldController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OnHoldController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateOnHoldTicket([FromForm]CreateOnHoldTicketCommand command)
        {
            try
            {
                if (User.Identity is ClaimsIdentity identity && Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                {
                    command.Added_By = userId;
                }
                var results = await _mediator.Send(command);
                if (results.IsFailure)
                {
                    return BadRequest(results);
                }
                return Ok(results);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }

        }

        [HttpPut("approval")]
        public async Task<IActionResult> ApprovalOnHold([FromBody] ApprovalOnHoldCommand command)
        {
            try
            {
                if (User.Identity is ClaimsIdentity identity)
                {
                    var userRole = identity.FindFirst(ClaimTypes.Role);
                    if (userRole != null)
                    {
                        command.Role = userRole.Value;
                    }

                    if (Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                    {
                        command.Users = userId;
                        command.Transacted_By = userId;
                    }
                }
                var results = await _mediator.Send(command);
                if (results.IsFailure)
                {
                    return BadRequest(results);
                }
                return Ok(results);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }

        }

        [HttpGet("page")]
        public async Task<IActionResult> GetOnHold([FromQuery] GetOnHoldQuery query)
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

                var onHoldTicket = await _mediator.Send(query);

                Response.AddPaginationHeader(

                onHoldTicket.CurrentPage,
                onHoldTicket.PageSize,
                onHoldTicket.TotalCount,
                onHoldTicket.TotalPages,
                onHoldTicket.HasPreviousPage,
                onHoldTicket.HasNextPage

                );

                var result = new
                {
                    onHoldTicket,
                    onHoldTicket.CurrentPage,
                    onHoldTicket.PageSize,
                    onHoldTicket.TotalCount,
                    onHoldTicket.TotalPages,
                    onHoldTicket.HasPreviousPage,
                    onHoldTicket.HasNextPage
                };

                var successResult = Result.Success(result);


                return Ok(successResult);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }

        }

        [HttpPut("resume")]
        public async Task<IActionResult> ResumeOnHoldTicket([FromBody]ResumeOnHoldTicketCommand command)
        {
            try
            {
                if (User.Identity is ClaimsIdentity identity && Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                {
                    command.Transacted_By = userId;
                }
                var results = await _mediator.Send(command);
                if (results.IsFailure)
                {
                    return BadRequest(results);
                }
                return Ok(results);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }

        }

        [HttpDelete("cancel")]
        public async Task<IActionResult> CancelOnHold([FromBody] CancelOnHoldCommand command)
        {
            try
            {
                if (User.Identity is ClaimsIdentity identity && Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                {
                    command.Transacted_By = userId;
                }
                var results = await _mediator.Send(command);
                if (results.IsFailure)
                {
                    return BadRequest(results);
                }
                return Ok(results);

            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPut("reject")]
        public async Task<IActionResult> RejectOnHold([FromBody] RejectOnHoldCommand command)
        {
            try
            {

                if (User.Identity is ClaimsIdentity identity)
                {
                    var userRole = identity.FindFirst(ClaimTypes.Role);
                    if (userRole != null)
                    {
                        command.Role = userRole.Value;
                    }

                    if (Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                    {
                        command.RejectOnHold_By = userId;
                        command.Transacted_By = userId;
                    }
                }

                var results = await _mediator.Send(command);
                if (results.IsFailure)
                {
                    return BadRequest(results);
                }
                return Ok(results);

            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

    }
}
