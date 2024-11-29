using MakeItSimple.WebApi.Common;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Ticketing.ClosedTicketConcern.AddClosingTicket
{
    public class AddNewClosingTicketCommand : IRequest<Result>
    {
        public string Closed_Remarks { get; set; }
        public Guid? Modified_By { get; set; }
        public Guid? Added_By { get; set; }
        public int? TicketConcernId { get; set; }
        public int? ClosingTicketId { get; set; }
        public string Resolution { get; set; }
        public string Notes { get; set; }

        public List<AddClosingTicketTechnician> AddClosingTicketTechnicians { get; set; }
        public class AddClosingTicketTechnician
        {
            public int? TicketTechnicianId { get; set; }
            public Guid? Technician_By { get; set; }
        }

        public string Modules { get; set; }
        public List<ClosingTicketCategory> ClosingTicketCategories { get; set; }
        public class ClosingTicketCategory
        {
            public int? TicketCategoryId { get; set; }
            public int? CategoryId { get; set; }
        }

        public List<ClosingSubTicketCategory> ClosingSubTicketCategories { get; set; }
        public class ClosingSubTicketCategory
        {
            public int? TicketSubCategoryId { get; set; }
            public int? SubCategoryId { get; set; }
        }

        public List<AddClosingAttachment> AddClosingAttachments { get; set; }

        public class AddClosingAttachment
        {
            public int? TicketAttachmentId { get; set; }
            public IFormFile Attachment { get; set; }

        }
    }

}
