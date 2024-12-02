using MakeItSimple.WebApi.Common.Extension;
using MakeItSimple.WebApi.Common;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.ReceiverSetup.GetReceiver;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.Phase_Two.Pms_Form_Setup.Create_Pms_Form.CreatePmsForm;
using MakeItSimple.WebApi.DataAccessLayer.Features.Setup.Phase_Two.Pms_Form_Setup.Get_Pms_Form;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.Phase_Two.Pms_Form_Setup.Get_Pms_Form.GetPmsForm;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_Two.Pms_Form_Setup.Update_Pms_Form.UpdatePmsForm;
using MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_Two.Pms_Form_Setup.Update_Pms_Form_Status;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_Two.Pms_Form_Setup.Update_Pms_Form_Status.UpdatePmsFormStatus;

namespace MakeItSimple.WebApi.Controllers.Setup.Phase_two.Pms_Form_Controller
{
    [Route("api/pms-form")]
    [ApiController]
    public class PmsFormController : ControllerBase
    {
        private readonly IMediator mediator;

        public PmsFormController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreatePmsForm([FromBody] CreatePmsFormCommand command)
        {
            try
            {
                if (User.Identity is ClaimsIdentity identity && Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                {
                    command.Added_By = userId;
                }
                var result = await mediator.Send(command);

                return result.IsFailure ? BadRequest(result) : Ok(result);

            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }


        [HttpGet("page")]
        public async Task<IActionResult> GetPmsForm([FromQuery] GetPmsFormQuery query)
        {
            try
            {
                var pmsForm = await mediator.Send(query);

                Response.AddPaginationHeader(

                pmsForm.CurrentPage,
                pmsForm.PageSize,
                pmsForm.TotalCount,
                pmsForm.TotalPages,
                pmsForm.HasPreviousPage,
                pmsForm.HasNextPage

                );

                var result = new
                {
                    pmsForm,
                    pmsForm.CurrentPage,
                    pmsForm.PageSize,
                    pmsForm.TotalCount,
                    pmsForm.TotalPages,
                    pmsForm.HasPreviousPage,
                    pmsForm.HasNextPage
                };

                var successResult = Result.Success(result);
                return Ok(successResult);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdatePmsForm([FromRoute] int id, [FromBody] UpdatePmsFormCommand command)
        {
            try
            {
                command.Id = id;

                if (User.Identity is ClaimsIdentity identity && Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                {
                    command.Modified_By = userId;
                }
                var result = await mediator.Send(command);

                return result.IsFailure ? BadRequest(result) : Ok(result);

            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPatch("archived/{id}")]
        public async Task<IActionResult> UpdatePmsFormStatus([FromRoute]int id)
        {
            try
            {
                var command = new UpdatePmsFormStatusCommand
                {
                    Id = id
                };

                var result = await mediator.Send(command);

                return result.IsFailure ? BadRequest(result) : Ok(result);

            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }


    }
}
