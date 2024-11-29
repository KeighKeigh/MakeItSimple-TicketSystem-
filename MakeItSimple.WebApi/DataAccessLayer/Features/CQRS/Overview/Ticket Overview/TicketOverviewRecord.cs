namespace MakeItSimple.WebApi.DataAccessLayer.Features.Overview.Ticket_Overview
{
    public partial class TicketOverview
    {
        public record TicketOverviewRecord
        {
            public int? TicketConcernId { get; set; }
            public string Request_Type { get; set; }
            public string Requestor_Name { get; set; }
            public int? BackJobId { get; set; }
            public int? Personnel_Unit { get; set; }
            public Guid? Personnel_Id { get; set; }
            public string Personnel { get; set; }
            public string Concerns { get; set; }

            public string Channel_Name { get; set; }
            public string TicketCategoryDescriptions { get; set; }
            public string TicketSubCategoryDescriptions { get; set; }
            public DateTime? Date_Needed { get; set; }
            public string Contact_Number { get; set; }
            public string Notes { get; set; }
            public DateTime? Transaction_Date { get; set; }
            public DateTime? Target_Date { get; set; }
            public string Ticket_Status { get; set; }
            public string Closed_Status { get; set; }
            public string Remarks { get; set; }
        }
    }
}
