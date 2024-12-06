using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Extension;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_Two.Pms_Form_Setup.Update_Pms_Form_Status.UpdatePmsFormStatus;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_Two.Pms_Questionaire_Module_Setup.CreatePmsQuestionaireModule;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_Two.Pms_Questionaire_Module_Setup.Get_Pms_Questionaire_Module.GetPmsQuestionaireModule;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_Two.Pms_Questionaire_Module_Setup.Update_Pms_Questionaire_Module.UpdatePmsQuestionaireModule;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_Two.Pms_Questionaire_Module_Setup.Update_Pms_Questionaire_Module.UpdatePmsQuestionaireModuleStatus;

namespace MakeItSimple.WebApi.Controllers.Setup.Phase_two.Pms_Questionaire_Module_Controller
{
    [Route("api/pms-questionaire-module")]
    [ApiController]
    public class PmsQuestionaireModuleController : ControllerBase
    {
        private readonly IMediator mediator;

        public PmsQuestionaireModuleController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreatePmsQuestionaireModule([FromBody] CreatePmsQuestionaireModuleCommand command)
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
        public async Task<IActionResult> GetPmsQuestionaireModule([FromQuery] GetPmsQuestionaireModuleQuery query)
        {
            try
            {
                var pmsQModule = await mediator.Send(query);

                Response.AddPaginationHeader(

                pmsQModule.CurrentPage,
                pmsQModule.PageSize,
                pmsQModule.TotalCount,
                pmsQModule.TotalPages,
                pmsQModule.HasPreviousPage,
                pmsQModule.HasNextPage

                );
                var result = new
                {
                    pmsQModule,
                    pmsQModule.CurrentPage,
                    pmsQModule.PageSize,
                    pmsQModule.TotalCount,
                    pmsQModule.TotalPages,
                    pmsQModule.HasPreviousPage,
                    pmsQModule.HasNextPage
                };

                var successResult = Result.Success(result);
                return Ok(successResult);

            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdatePmsQuestionModule([FromRoute] int id, [FromBody] UpdatePmsQuestionaireModuleCommand command)
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
        public async Task<IActionResult> UpdatePmsQuestionModuleStatus([FromRoute] int id)
        {
            try
            {
                var command = new UpdatePmsQuestionaireModuleStatusCommand
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
