namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.OnHoldTicket.Get_OnHold
{
    public partial class GetOnHold
    {
        public record GetOnHoldReports
        {
            public int? TicketConcernId { get; set; }
            public int TicketOnHoldId { get; set; }
            public int? ChannelId { get; set; }
            public string Channel_Name { get; set; }
            public Guid? UserId { get; set; }
            public string Fullname { get; set; }
            public string Concern_Details { get; set; }
            public DateTime Transacted_At { get; set; }
            public string Reason { get; set; }
            public bool? IsHold { get; set; }
            public List<GetOnHoldTicketCategory> GetOnHoldTicketCategories { get; set; }

            public class GetOnHoldTicketCategory
            {
                public int? TicketCategoryId { get; set; }
                public int? CategoryId { get; set; }
                public string Category_Description { get; set; }

            }

            public List<GetOnHoldTicketSubCategory> GetOnHoldTicketSubCategories { get; set; }

            public class GetOnHoldTicketSubCategory
            {
                public int? TicketSubCategoryId { get; set; }
                public int? SubCategoryId { get; set; }
                public string SubCategory_Description { get; set; }
            }

            public DateTime? Resume_At { get; set; }
            public string OnHold_Remarks { get; set; }
            public bool IsReject_OnHold { get; set; }
            public DateTime? Reject_OnHold_At { get; set; }
            public string Reject_OnHold_By { get; set; }
            public string Reject_Remarks { get; set; }
            public string OnHold_Status { get; set; }

            public List<OnHoldAttachment> OnHoldAttachments { get; set; }
            public class OnHoldAttachment
            {
                public int? TicketAttachmentId { get; set; }
                public string Attachment { get; set; }
                public string FileName { get; set; }
                public decimal? FileSize { get; set; }
                public string Added_By { get; set; }
                public DateTime Created_At { get; set; }
                public string Modified_By { get; set; }
                public DateTime? Updated_At { get; set; }
            }
        }
    }
}
