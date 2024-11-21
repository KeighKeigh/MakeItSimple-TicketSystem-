using MakeItSimple.WebApi.Common.Pagination;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.OnHoldTicket.Get_OnHold
{
    public partial class GetOnHold
    {
        public class GetOnHoldQuery : UserParams, IRequest<PagedList<GetOnHoldReports>>
        {

            public Guid? UserId { get; set; }
            public string UserType { get; set; }
            public string Role { get; set; }
            public bool? IsHold { get; set; }
            public bool? IsReject { get; set; }
            public string Search { get; set; }
        }
    }
}
