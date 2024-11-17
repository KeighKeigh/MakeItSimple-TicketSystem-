using MakeItSimple.WebApi.Common;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.AssignTicket
{
    public partial class AddRequestConcernReceiver
    {
        public class AddRequestConcernReceiverCommand : IRequest<Result>
        {
            public int? TicketConcernId { get; set; }
            public string Concern { get; set; }
            public int? ChannelId { get; set; }
            public List<RequestorTicketCategory> RequestorTicketCategories { get; set; }
            public class RequestorTicketCategory
            {
                public int? TicketCategoryId { get; set; }
                public int? CategoryId { get; set; }

            }

            public List<RequestorTicketSubCategory> RequestorTicketSubCategories { get; set; }
            public class RequestorTicketSubCategory
            {
                public int? TicketSubCategoryId { get; set; }
                public int? SubCategoryId { get; set; }

            }
            public Guid? Requestor_By { get; set; }
            public Guid? Added_By { get; set; }
            public Guid? Modified_By { get; set; }
            public Guid? UserId { get; set; }

            public string Notes { get; set; }
            public string Contact_Number { get; set; }
            public string Request_Type { get; set; }
            public int? BackJobId { get; set; }

            public DateTime? DateNeeded { get; set; }

            public DateTime Start_Date { get; set; }
            public DateTime Target_Date { get; set; }
            public string Role { get; set; }
            public string Remarks { get; set; }

            public List<ConcernAttachment> ConcernAttachments { get; set; }

            public class ConcernAttachment
            {
                public int? TicketAttachmentId { get; set; }
                public IFormFile Attachment { get; set; }
            }

        }
    }
}
