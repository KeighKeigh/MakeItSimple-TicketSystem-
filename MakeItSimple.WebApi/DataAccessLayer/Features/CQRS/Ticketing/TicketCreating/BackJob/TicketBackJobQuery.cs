
using MakeItSimple.WebApi.Common;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.BackJob
{
    public partial class TicketBackJob
    {
        public class TicketBackJobQuery : IRequest<Result>
        {
            public string Search {  get; set; }

            public Guid ? UserId { get; set; }
        
        }
    }
}
