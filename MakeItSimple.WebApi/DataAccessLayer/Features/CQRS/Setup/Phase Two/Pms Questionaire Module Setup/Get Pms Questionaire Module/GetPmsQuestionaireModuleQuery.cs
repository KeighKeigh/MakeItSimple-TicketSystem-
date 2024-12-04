using MakeItSimple.WebApi.Common.Pagination;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_Two.Pms_Questionaire_Module_Setup.Get_Pms_Questionaire_Module
{
    public partial class GetPmsQuestionaireModule 
    {
        public class GetPmsQuestionaireModuleQuery : UserParams, IRequest<PagedList<GetPmsQuestionaireModuleResult>>
        {
            public string Search { get; set; }
            public bool? Is_Archived { get; set; }
            public string Orders { get; set; }

        }
    }
}
