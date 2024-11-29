using MakeItSimple.WebApi.Common;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosedTicketConcern.RejectClosing
{
    public partial class RejectClosingTicket
    {
        public class RejectClosingTicketCommand : IRequest<Result>
        {
            public Guid? RejectClosed_By { get; set; }
            public Guid? Transacted_By { get; set; }
            public string Reject_Remarks { get; set; }
            public int? ClosingTicketId { get; set; }
            public string Modules { get; set; }

        }
    }
}
