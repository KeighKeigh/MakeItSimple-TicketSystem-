using MakeItSimple.WebApi.Common;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.OnHoldTicket.Reject_On_Hold
{
    public partial class RejectOnHold
    {
        public class RejectOnHoldCommand : IRequest<Result>
        {
            public Guid? RejectOnHold_By { get; set; }
            public Guid? Transacted_By { get; set; }
            public string Role { get; set; }
            public int OnHoldTicketId { get; set; }
            public string Reject_Remarks { get; set; }
        }
    }
}
