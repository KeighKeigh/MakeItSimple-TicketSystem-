using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_Two.Pms_Questionaire_Setup.Create_Pms_Questionaire.CreatePmsQuestion;


namespace MakeItSimple.WebApi.Controllers.Setup.Phase_two.Pms_Question_Controller
{
    [Route("api/pms-question")]
    [ApiController]
    public class PmsQuestionController : ControllerBase
    {                                          

        private readonly IMediator mediator;
        private readonly MisDbContext context;

        public PmsQuestionController(IMediator mediator, MisDbContext context)
        {
            this.mediator = mediator;
            this.context = context;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreatePmsQuestion([FromBody] CreatePmsQuestionCommand command)
        {

            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {


                if (User.Identity is ClaimsIdentity identity && Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                {
                    command.Added_By = userId;
                }
                var result = await mediator.Send(command);

                if (result.IsFailure)
                {
                    //await unitOfWork.RollBackTransaction();
                    await transaction.RollbackAsync();
                    return BadRequest(result);
                }

                //await unitOfWork.CommitTransaction();
                await transaction.CommitAsync();
                return Ok(result);

            }
            catch (Exception ex)
            {
                //await unitOfWork.RollBackTransaction();
                await transaction.RollbackAsync();
                return Conflict(ex.Message);
            }
        }

    }
}
