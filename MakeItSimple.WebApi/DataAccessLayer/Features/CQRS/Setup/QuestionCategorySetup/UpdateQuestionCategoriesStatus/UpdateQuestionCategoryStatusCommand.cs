using MakeItSimple.WebApi.Common;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.QuestionCategorySetup.UpdateQuestionCategoriesStatus
{
    public partial class UpdateQuestionCategoryStatus
    {
        public class UpdateQuestionCategoryStatusCommand : IRequest<Result>
        {
            public int Id { get; set; }
        }
    }
}
