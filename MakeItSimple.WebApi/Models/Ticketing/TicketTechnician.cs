namespace MakeItSimple.WebApi.Models.Ticketing
{
    public class TicketTechnician
    {
        public int Id { get; set; }
        public int ClosingTicketId { get; set; }
        public virtual ClosingTicket ClosingTicket { get; set; }
        public Guid ? TechnicianBy {  get; set; }
        public virtual User TechnicianByUser { get; set; }
    }
}
