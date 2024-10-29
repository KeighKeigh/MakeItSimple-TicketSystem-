using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MediatR;
using NuGet.Protocol.Plugins;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Export.OnHoldExport
{
    public class OnHoldTicketExport
    {
        public record OnHoldTicketExportResult
        {
            public int TicketConcernId { get; set; }
            public string Concerns { get; set; }
            public string Reason { get; set; }
            public string Added_By { get; set; }
            public DateTime Created_At { get; set; }
            public bool? IsHold { get; set; }
            public DateTime? Resume_At { get; set; }
        }

        public class OnHoldTicketExportQuery : IRequest<Unit>
        {
            public string Search { get; set; }
            public int? Unit { get; set; }
            public Guid? UserId { get; set; }
            public DateTime? Date_From { get; set; }
            public DateTime? Date_To { get; set; }

        }

        public class Handler : IRequestHandler<OnHoldTicketExportQuery, Unit>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Unit> Handle(OnHoldTicketExportQuery request, CancellationToken cancellationToken)
            {

                 
               

                return Unit.Value;
            }
        }

    }
}
