using MakeItSimple.WebApi.Common;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket.TransferUser
{
    public partial class TransferTicketUser
    {
        public class TransferTicketUserCommand : IRequest<Result>
        {
            public Guid Transfer_By {  get; set; }
            public int TicketConcernId { get; set; }
        }
    }
}
