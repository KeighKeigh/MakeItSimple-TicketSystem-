using MakeItSimple.WebApi.Common.Pagination;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Overview.Ticket_Overview
{
    public partial class TicketOverview
    {
        public class TicketOverviewQuery : UserParams, IRequest<PagedList<TicketOverviewRecord>>
        {
            public Guid? UserId { get; set; }
            public string Role {  get; set; }
            public string UserType { get; set; }
            public string Search { get; set; }

        }
    }
}
