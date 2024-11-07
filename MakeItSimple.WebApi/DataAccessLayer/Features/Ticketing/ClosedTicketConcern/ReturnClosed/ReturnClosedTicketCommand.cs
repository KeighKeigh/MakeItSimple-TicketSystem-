using MakeItSimple.WebApi.Common;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosedTicketConcern.ReturnClosed
{
    public partial class ReturnClosedTicket
    {
        public class ReturnClosedTicketCommand : IRequest<Result>
        {
            public int? RequestConcernId { get; set; }
            public string Remarks { get; set; }

            public Guid? Added_By { get; set; }

            public string Modules { get; set; }

            public List<ReturnTicketAttachment> ReturnTicketAttachments { get; set; }
            public class ReturnTicketAttachment
            {
                public int? TicketAttachmentId { get; set; }
                public IFormFile Attachment { get; set; }
            }
        }
    }
}
