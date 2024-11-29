using MakeItSimple.WebApi.Common;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketingNotification.TicketsNotification
{
    public partial class TicketingNotification
    {
        public class TicketingNotificationCommand : IRequest<Result>
        {
            public Guid UserId { get; set; }
            public string Role { get; set; }


        }
    }
}
