using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Export.OnHoldExport
{
    public partial class OnHoldTicketExport
    {
        public class OnHoldTicketExportQuery : IRequest<Unit>
        {
            public string Search { get; set; }
            public int? Unit { get; set; }
            public Guid? UserId { get; set; }
            public DateTime? Date_From { get; set; }
            public DateTime? Date_To { get; set; }

        }

    }
}
