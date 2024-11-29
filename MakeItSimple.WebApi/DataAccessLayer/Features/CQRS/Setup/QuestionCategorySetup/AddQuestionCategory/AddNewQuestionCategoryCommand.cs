using MakeItSimple.WebApi.Common;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.QuestionCategorySetup.AddQuestionCategory
{
    public partial class AddNewQuestionCategory
    {
        public class AddNewQuestionCategoryCommand : IRequest<Result>
        {
            public int FormId { get; set; }
            public string Question_Category_Name { get; set; }
            public Guid ? Added_By { get; set; }

        }
    }
}
