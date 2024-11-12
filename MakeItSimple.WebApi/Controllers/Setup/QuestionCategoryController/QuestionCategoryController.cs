using MakeItSimple.WebApi.Common.Extension;
using MakeItSimple.WebApi.Common;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.QuestionCategorySetup.AddQuestionCategory.AddNewQuestionCategory;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.QuestionCategorySetup.UpdateQuestionCategories.UpdateQuestionCategory;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.QuestionCategorySetup.GetQuestionCategories.GetQuestionCategory;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.QuestionCategorySetup.UpdateQuestionCategoriesStatus.UpdateQuestionCategoryStatus;
using System.Security.Claims;

namespace MakeItSimple.WebApi.Controllers.Setup.QuestionCategoryController
{
    [Route("api/question-category")]
    [ApiController]
    public class QuestionCategoryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public QuestionCategoryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("create")]
        public async Task<IActionResult> AddNewQuestionCategory([FromBody] AddNewQuestionCategoryCommand command)
        {
            try
            {
                if (User.Identity is ClaimsIdentity identity && Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                {
                    command.Added_By = userId;
                }

                var result = await _mediator.Send(command);

                if (result.IsFailure)
                    return BadRequest(result);

                return Ok(result);

            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateQuestionCategories([FromBody] UpdateQuestionCategoryCommand command)
        {
            try
            {
                if (User.Identity is ClaimsIdentity identity && Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                {
                    command.Modified_By = userId;
                }

                var result = await _mediator.Send(command);
                if (result.IsFailure)
                    return BadRequest(result);

                return Ok(result);

            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpGet("page")]
        public async Task<IActionResult> GetQuestionCategories([FromQuery] GetQuestionCategoryQuery query)
        {
            try
            {

                var questionCategory = await _mediator.Send(query);

                Response.AddPaginationHeader(

                questionCategory.CurrentPage,
                questionCategory.PageSize,
                questionCategory.TotalCount,
                questionCategory.TotalPages,
                questionCategory.HasPreviousPage,
                questionCategory.HasNextPage

                );

                var result = new
                {
                    questionCategory,
                    questionCategory.CurrentPage,
                    questionCategory.PageSize,
                    questionCategory.TotalCount,
                    questionCategory.TotalPages,
                    questionCategory.HasPreviousPage,
                    questionCategory.HasNextPage
                };

                var successResult = Result.Success(result);
                return Ok(successResult);

            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPatch("status/{id}")]
        public async Task<IActionResult> UpdateQuestionCategoriesStatus([FromRoute] int id)
        {
            try
            {
                var command = new UpdateQuestionCategoryStatusCommand
                {
                    Id = id
                };

                var result = await _mediator.Send(command);

                if (result.IsFailure)
                    return BadRequest(result);

                return Ok(result);

            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

    }
}
