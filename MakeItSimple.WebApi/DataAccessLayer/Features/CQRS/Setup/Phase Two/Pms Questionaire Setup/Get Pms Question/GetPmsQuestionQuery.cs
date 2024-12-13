using MakeItSimple.WebApi.Common.Pagination;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_Two.Pms_Questionaire_Setup.Get_Pms_Question
{
    public partial class GetPmsQuestion
    {
        public class GetPmsQuestionQuery : UserParams, IRequest<PagedList<GetPmsQuestionResult>>
        {
            public string Search { get; set; }
            public bool? Is_Archived { get; set; }
            public string Orders { get; set; }

        }
    }
}
