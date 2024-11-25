namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketingNotification.TicketsNotification
{
    public partial class TicketingNotification
    {
        public class TicketingNotifResult
        {
            public int AllRequestTicketNotif { get; set; }
            public int ForTicketNotif { get; set; }
            public int CurrentlyFixingNotif { get; set; }
            public int NotConfirmNotif { get; set; }
            public int DoneNotif { get; set; }
            public int ReceiverForApprovalNotif { get; set; }
            public int AllTicketNotif { get; set; }
            public int OpenTicketNotif { get; set; }
            public int ForTransferNotif { get; set; }
            public int ForCloseNotif { get; set; }
            public int ForOnHoldNotif { get; set; }
            public int OnHold { get; set; }
            public int NotConfirmCloseNotif { get; set; }
            public int ClosedNotif { get; set; }
            public int ForApprovalOnHoldNotif { get; set; }
            public int ForApprovalTransferNotif { get; set; }
            public int ForApprovalClosingNotif { get; set; }

        }
    }
}
