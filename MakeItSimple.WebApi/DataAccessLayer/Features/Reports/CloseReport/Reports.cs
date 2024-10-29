namespace MakeItSimple.WebApi.DataAccessLayer.Features.Reports.CloseReport
{
    public partial class TicketReports
    {
        public record class Reports
        {
            public string Year { get; set; }
            public string Month { get; set; }
            public string Start_Date { get; set; }
            public string End_Date { get; set; }
            public string Personnel { get; set; }
            public int Ticket_Number { get; set; }
            public string Description { get; set; }
            public string Target_Date { get; set; }
            public string Actual { get; set; }
            public int Varience { get; set; }
            public decimal? Efficeincy { get; set; }
            public string Status { get; set; }
            public string Remarks { get; set; }
            public string Category { get; set; }

        }

    }
}
