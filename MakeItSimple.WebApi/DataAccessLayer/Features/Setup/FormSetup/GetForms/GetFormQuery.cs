using MakeItSimple.WebApi.Common.Pagination;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.FormSetup.GetForms
{
    public partial class GetForm
    {
        public class GetFormQuery : UserParams, IRequest<PagedList<GetFormResult>>
        {
            public string Search { get; set; }
            public bool? Status { get; set; }

        }
    }
}
