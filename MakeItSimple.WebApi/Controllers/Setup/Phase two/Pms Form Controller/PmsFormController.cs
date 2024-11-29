using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.Phase_Two.Pms_Form_Setup.Create_Pms_Form.CreatePmsForm;

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
    }
}
