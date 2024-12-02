using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Extension;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Ticketing.TicketCreating.AddCommentNotificationValidator;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.AddRequest.AddRequestConcern;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.AssignTicket.AddRequestConcernReceiver;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Ticketing.TicketCreating.AddTicketComment;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.CancelTicket.CancelRequestConcern;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.GetAttachment.GetRequestAttachment;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.GetConcernTicket.GetRequestorTicketConcern;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Ticketing.TicketCreating.GetTicketComment;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.RemoveTicketAttachment.RemoveTicketAttachment;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Ticketing.TicketCreating.RemoveTicketComment;
using Microsoft.AspNetCore.SignalR;
using MakeItSimple.WebApi.Common.SignalR;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Ticketing.TicketCreating.DownloadImageTicketing;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.ViewImage.ViewTicketImage;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.ApprovalTicket.RequestApprovalReceiver;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.BackJob.TicketBackJob;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Ticketing.TicketCreating.ViewMultipleSubCategories.ViewMultipleSubCategory;



namespace MakeItSimple.WebApi.Controllers.Ticketing
{
    [Route("api/request-concern")]
    [ApiController]
    public class RequestConcernController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly TimerControl _timerControl;

        public RequestConcernController(IMediator mediator , TimerControl timerControl )
        {
            _mediator = mediator;
            _timerControl = timerControl;
        }


        [HttpPost("add-request-concern")]
        public async Task<IActionResult> AddRequestConcern([FromForm]AddRequestConcernCommand command)
        {
            try
            {
                if (User.Identity is ClaimsIdentity identity && Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                {
                    command.Modified_By = userId;
                    command.Added_By = userId;
                    //command.UserId = userId;
                   
                }
                var result = await _mediator.Send(command);
                if(result.IsFailure)
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        } 


        [HttpGet("backjob")]
        public async Task<IActionResult> TicketBackJob([FromQuery] TicketBackJobQuery command)
        {
            try
            {
                if (User.Identity is ClaimsIdentity identity && Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                {
                    command.UserId = userId;

                }
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }


        [HttpGet("multiple-sub-category")]
        public async Task<IActionResult> ViewMultipleSubCategory([FromQuery] ViewMultipleSubCategoryQuery command)
        {
            try
            {

                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPut("cancel-request")]
        public async Task<IActionResult> CancelRequestConcern([FromBody] CancelRequestConcernCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                if (result.IsFailure)
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPut("remove-attachment")]
        public async Task<IActionResult> RemoveTicketAttachment([FromBody] RemoveTicketAttachmentCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                if(result.IsFailure)
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }


        [HttpPost("add-ticket-concern")]
        public async Task<IActionResult> AddRequestConcernReceiver([FromForm]AddRequestConcernReceiverCommand command)
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
                        command.Modified_By = userId;

                    }
                }
                var result = await _mediator.Send(command);
                if (result.IsFailure)
                {
                    return BadRequest(result);
                }
                return Ok(result);


            }
            catch(Exception ex)
            {
                return Conflict(ex.Message);
            }
        }


        [HttpGet("request-attachment")]
        public async Task<IActionResult> GetRequestAttachment([FromQuery] GetRequestAttachmentQuery query)
        {
            try
            {
                var results = await _mediator.Send(query);
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

        [HttpGet("requestor-page")]
        public async Task<IActionResult> GetRequestorTicketConcern([FromQuery] GetRequestorTicketConcernQuery query)
        {
            try
            {
                if (User.Identity is ClaimsIdentity identity)
                {

                    if (Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                    {
                        query.UserId = userId;
                        
                    }
                    var userRole = identity.FindFirst(ClaimTypes.Role);
                    if (userRole != null)
                    {
                        query.Role = userRole.Value;
                    }
                }

                var requestConcern = await _mediator.Send(query);

                Response.AddPaginationHeader(

                requestConcern.CurrentPage,
                requestConcern.PageSize,
                requestConcern.TotalCount,
                requestConcern.TotalPages,
                requestConcern.HasPreviousPage,
                requestConcern.HasNextPage

                );

                var result = new
                {
                    requestConcern,
                    requestConcern.CurrentPage,
                    requestConcern.PageSize,
                    requestConcern.TotalCount,
                    requestConcern.TotalPages,
                    requestConcern.HasPreviousPage,
                    requestConcern.HasNextPage
                }; 

                var successResult = Result.Success(result);

                return Ok(successResult);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }


        [HttpPut("approval-request-receiver")]
        public async Task<IActionResult> RequestApprovalReceiver([FromBody] RequestApprovalReceiverCommand command)
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
                        command.UserId = userId;
                        command.Approved_By = userId;

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

        [HttpPost("add-comment")]
        public async Task<IActionResult> AddTicketComment([FromForm] AddTicketCommentCommand command)
        {
            try
            {
                if (User.Identity is ClaimsIdentity identity && Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                {
                    command.Modified_By = userId;
                    command.Added_By = userId;
                    command.UserId = userId;

                }
                var result = await _mediator.Send(command);
                if (result.IsFailure)
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPut("remove-comment")]
        public async Task<IActionResult> RemoveTicketComment([FromBody] RemoveTicketCommentCommand command)
        {
            try
            {
                if (User.Identity is ClaimsIdentity identity && Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                {
                    command.UserId = userId;

                }
                var result = await _mediator.Send(command);
                if (result.IsFailure)
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpGet("view-comment")]
        public async Task<IActionResult> GetTicketComment([FromQuery]GetTicketCommentQuery query)
        {
            try
            {
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch(Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPost("add-comment-view")]
        public async Task<IActionResult> AddCommentNotificationValidator([FromBody] AddCommentNotificationValidatorCommand command)
        {
            try
            {

                if (User.Identity is ClaimsIdentity identity && Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                {
                    command.UserId = userId;
                    command.Added_By = userId;  

                }    
                var result = await _mediator.Send(command);
                if(result.IsFailure)
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }


        [HttpGet("download/{id}")]
        public async Task<IActionResult> DownloadImageTicketing(int id)
        {
            try
            {
                var query = new DownloadImageTicketingCommand
                {
                    TicketAttachmentId = id
                };

                var result = await _mediator.Send(query);
                if (result.IsSuccess)
                {
                    var fileResult = result is Result<FileStreamResult> fileStreamResult ? fileStreamResult.Value : null;

                    if (fileResult != null)
                    {
                        return fileResult;
                    }

                    return BadRequest(result);
                }

                return BadRequest(new { ErrorCode = result.Error.Code, ErrorMessage = result.Error.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("view/{id}")]
        public async Task<IActionResult> ViewTicketImage(int id)
        {
            try
            {
                var query = new ViewTicketImageCommand
                {
                    TicketAttachmentId = id
                };

                var result = await _mediator.Send(query);
                if (result.IsSuccess)
                {
                    var fileResult = result is Result<FileStreamResult> fileStreamResult ? fileStreamResult.Value : null;

                    if (fileResult != null)
                    {
                        Response.Headers.Add("Content-Disposition", "inline");
                        return fileResult;
                    }

                    return BadRequest(result);
                }

                return BadRequest(new { ErrorCode = result.Error.Code, ErrorMessage = result.Error.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


    }
}
