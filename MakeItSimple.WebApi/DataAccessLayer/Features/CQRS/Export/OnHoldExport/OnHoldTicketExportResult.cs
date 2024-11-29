namespace MakeItSimple.WebApi.DataAccessLayer.Features.Export.OnHoldExport
{
    public partial class OnHoldTicketExport
    {
        public record OnHoldTicketExportResult
        {
            public Guid? UserId { get; set; }
            public int? Unit { get; set; }
            public int TicketConcernId { get; set; }
            public string Concerns { get; set; }
            public string Reason { get; set; }
            public string Added_By { get; set; }
            public DateTime Created_At { get; set; }
            public bool? IsHold { get; set; }
            public DateTime? Resume_At { get; set; }
        }

    }
}
