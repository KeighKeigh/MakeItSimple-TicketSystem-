using MakeItSimple.WebApi.Common;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Ticketing.TicketCreating
{
    public partial class DownloadImageTicketing
    {
        public class DownloadImageTicketingCommand : IRequest<Result>
        {
            public int TicketAttachmentId { get; set; }
        }
    }
}
