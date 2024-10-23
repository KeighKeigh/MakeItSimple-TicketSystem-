using MakeItSimple.WebApi.Common;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.OnHoldTicket.ResumeOnHold
{
    public partial class ResumeOnHoldTicket
    {
        public class ResumeOnHoldTicketCommand : IRequest<Result>
        {
            public int TicketOnHoldId { get; set; }
            public Guid? Transacted_By { get; set; }
        }
    }
}
