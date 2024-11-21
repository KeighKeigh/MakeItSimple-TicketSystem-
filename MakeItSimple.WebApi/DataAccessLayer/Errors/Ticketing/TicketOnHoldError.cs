using MakeItSimple.WebApi.Common;

namespace MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing
{
    public class TicketOnHoldError
    {
        public static Error TicketOnHoldIdNotExist() =>
        new Error("TicketOnHoldTicketError.TicketOnHoldIdNotExist", "On-hold concern not exist!");
    }
}
