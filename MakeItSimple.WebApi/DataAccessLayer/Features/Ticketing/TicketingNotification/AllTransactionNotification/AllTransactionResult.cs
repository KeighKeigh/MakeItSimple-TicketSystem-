namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketingNotification.AllTransactionNotification
{
    public partial class GetAllTransactionNotification
    {
        private class AllTransactionResult
        {
            public int Id { get; set; }
            public string Message { get; set; }
            public string Added_By { get; set; }
            public DateTime Created_At { get; set; }
            public string Receive_By { get; set; }
            public bool Is_Checked { get; set; }
            public string Modules { get; set; }
            public string Modules_Parameter { get; set; }
            public int? PathId { get; set; }
        }


    }
}
