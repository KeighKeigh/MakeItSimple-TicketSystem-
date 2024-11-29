namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.BackJob
{
    public partial class TicketBackJob
    {
        public record TicketBackJobResult
        {
            public int TicketConcernId { get; set; }
            public int Days_Closed { get; set; }
            public string Concern { get; set; }
        }
    }
}
