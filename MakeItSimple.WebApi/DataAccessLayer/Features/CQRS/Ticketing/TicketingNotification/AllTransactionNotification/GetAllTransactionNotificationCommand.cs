using MakeItSimple.WebApi.Common;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketingNotification.AllTransactionNotification
{
    public partial class GetAllTransactionNotification
    {
        public class GetAllTransactionNotificationCommand : IRequest<Result>
        {
            public Guid UserId { get; set; }
            public string Role { get; set; }


        }


    }
}
