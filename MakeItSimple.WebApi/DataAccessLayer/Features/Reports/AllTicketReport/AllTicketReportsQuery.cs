using MakeItSimple.WebApi.Common.Pagination;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Reports.AllTicketReport
{
    public partial class AllTicketReports
    {
        public class AllTicketReportsQuery : UserParams, IRequest<PagedList<AllTicketReportsResult>>
        {
            public string Search { get; set; }
            public int? Unit { get; set; }
            public Guid? UserId { get; set; }
            public DateTime? Date_From { get; set; }
            public DateTime? Date_To { get; set; }

        }

    }
}
