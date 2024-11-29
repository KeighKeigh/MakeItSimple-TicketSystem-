using MakeItSimple.WebApi.Common;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.OnHoldTicket.Cancel_On_Hold
{
    public partial class CancelOnHold
    {
        public class CancelOnHoldCommand : IRequest<Result>
        {
            public int OnHoldTicketId { get; set; }
            public Guid? Transacted_By { get; set; }
        }
    }
}
