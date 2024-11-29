using MakeItSimple.WebApi.Common;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket.RejectTransfer
{
    public partial class RejectTransferTicket
    {
        public class RejectTransferTicketCommand : IRequest<Result>
        {
            public Guid? RejectTransfer_By { get; set; }
            public Guid? Transacted_By { get; set; }
            public string Role { get; set; }
            public int TransferTicketId { get; set; }

            public string Modules { get; set; }

            public string Reject_Remarks { get; set; }
        }
    }
}
