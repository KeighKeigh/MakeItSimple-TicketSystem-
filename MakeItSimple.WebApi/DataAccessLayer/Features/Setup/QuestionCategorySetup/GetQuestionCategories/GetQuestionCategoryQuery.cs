using MakeItSimple.WebApi.Common.Pagination;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.QuestionCategorySetup.GetQuestionCategories
{
    public partial class GetQuestionCategory
    {
        public class GetQuestionCategoryQuery : UserParams, IRequest<PagedList<GetQuestionCategoryResult>>
        {
            public string Search {  get; set; }
            public bool? Status { get; set; }

        }
    }
}
