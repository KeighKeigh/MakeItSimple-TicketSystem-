using MakeItSimple.WebApi.Common;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.OnHoldTicket.Approval_On_Hold
{
    public partial class ApprovalOnHold
    {
        public class ApprovalOnHoldCommand : IRequest<Result>
        {
            public string Role { get; set; }
            public Guid? Users { get; set; }
            public Guid? Transacted_By { get; set; }
            public int OnHoldTicketId { get; set; }

        }

    }
}
