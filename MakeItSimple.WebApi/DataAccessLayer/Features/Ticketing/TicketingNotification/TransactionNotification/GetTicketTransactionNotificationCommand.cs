using MakeItSimple.WebApi.Common;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketingNotification.TransactionNotification
{
    public partial class GetTicketTransactionNotification
    {
        public class GetTicketTransactionNotificationCommand : IRequest<Result>
        {
            public Guid UserId { get; set; }
            public string Role { get; set; }

        }
    }
}
