namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket.GetTransfer
{
    public partial class GetTransferTicket
    {
        public class GetTransferTicketResult
        {

            public int? TicketConcernId { get; set; }
            public int? TransferTicketId { get; set; }
            public int? ChannelId { get; set; }
            public string Channel_Name { get; set; }
            public Guid? UserId { get; set; }
            public string Fullname { get; set; }
            public string Concern_Details { get; set; }

            public List<GetTransferTicketCategory> GetTransferTicketCategories { get; set; }

            public class GetTransferTicketCategory
            {
                public int? TicketCategoryId { get; set; }
                public int? CategoryId { get; set; }
                public string Category_Description { get; set; }

            }

            public List<GetTransferTicketSubCategory> GetTransferTicketSubCategories { get; set; }

            public class GetTransferTicketSubCategory
            {
                public int? TicketSubCategoryId { get; set; }
                public int? SubCategoryId { get; set; }
                public string SubCategory_Description { get; set; }
            }
            public DateTime? Current_Target_Date { get; set; }
            public DateTime? Target_Date { get; set; }
            public bool IsActive { get; set; }
            public string Transfer_By { get; set; }
            public string Transfer_To { get; set; }
            
            public DateTime? Transfer_At { get; set; }
            public string Transfer_Status { get; set; }
            public string Transfer_Remarks { get; set; }
            public string RejectTransfer_By { get; set; }
            public DateTime? RejectTransfer_At { get; set; }
            public string Reject_Remarks { get; set; }
            public string Remarks { get; set; }
            public string Added_By { get; set; }
            public DateTime Created_At { get; set; }
            public string Modified_By { get; set; }
            public DateTime? Updated_At { get; set; }

            public int? Approver_Level { get; set; }

            public List<TransferAttachment> TransferAttachments { get; set; }

            public class TransferAttachment
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
