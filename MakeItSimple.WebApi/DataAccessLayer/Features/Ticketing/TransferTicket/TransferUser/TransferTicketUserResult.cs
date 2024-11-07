namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket.TransferUser
{
    public partial class TransferTicketUser
    {
        public class TransferTicketUserResult
        {
            public Guid ? Id { get; set; }
            public string EmpId { get; set; }
            public string Fullname { get; set; }
            public string Channel_Name { get; set; }
        }
    }
}
