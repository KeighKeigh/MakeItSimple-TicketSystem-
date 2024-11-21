namespace MakeItSimple.WebApi.DataAccessLayer.Features.Reports.AllTicketReport
{
    public partial class AllTicketReports
    {
        public record AllTicketReportsResult
        {
            public int? TicketConcernId { get; set; }
            public string Request_Type { get; set; }
            public int? BackJobId { get; set; }
            public string Requestor_Name { get; set; }
            public string Company_Code { get; set; }
            public string Company_Name { get; set; }
            public string BusinessUnit_Code { get; set; }
            public string BusinessUnit_Name { get; set; }
            public string Department_Code { get; set; }
            public string Department_Name { get; set; }
            public string Unit_Code { get; set; }
            public string Unit_Name { get; set; }
            public string SubUnit_Code { get; set; }
            public string SubUnit_Name { get; set; }
            public string Location_Code { get; set; }
            public string Location_Name { get; set; }
            public int? Personnel_Unit {  get; set; }
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
            public string Remarks { get; set; }


        }

    }
}
