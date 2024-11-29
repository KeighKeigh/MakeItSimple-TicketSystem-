using MakeItSimple.WebApi.Common;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.QuestionCategorySetup.UpdateQuestionCategories
{
    public partial class UpdateQuestionCategory
    {
        public class UpdateQuestionCategoryCommand : IRequest<Result>
        {
            public int Id  { get; set; }
            public int FormId { get; set; }
            public string Question_Category_Name { get; set; }

            public Guid? Modified_By { get; set; }

        }
    }
}
