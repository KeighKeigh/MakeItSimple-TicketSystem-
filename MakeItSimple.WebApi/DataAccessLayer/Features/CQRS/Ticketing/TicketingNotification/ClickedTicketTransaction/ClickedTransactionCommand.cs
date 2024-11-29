using MakeItSimple.WebApi.Common;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketingNotification.ClickedTicketTransaction
{
    public partial class ClickedTransaction
    {
        public class ClickedTransactionCommand : IRequest<Result>
        {
            public int Id { get; set; }

        }
    }
}
