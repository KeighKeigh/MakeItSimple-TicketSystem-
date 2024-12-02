using MakeItSimple.WebApi.Common.Pagination;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.Phase_Two.Pms_Form_Setup.Get_Pms_Form
{
    public partial class GetPmsForm
    {
        public class GetPmsFormQuery : UserParams , IRequest<PagedList<GetPmsFormResult>> 
        {
            public string Search {  get; set; }
            public bool? Is_Archived { get; set; }
            public string Orders { get; set; }

        }
    }
}
