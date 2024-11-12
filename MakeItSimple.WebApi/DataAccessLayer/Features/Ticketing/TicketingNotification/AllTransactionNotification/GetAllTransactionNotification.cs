using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketingNotification.AllTransactionNotification
{
    public partial class GetAllTransactionNotification
    {

        public class Handler : IRequestHandler<GetAllTransactionNotificationCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(GetAllTransactionNotificationCommand request, CancellationToken cancellationToken)
            {

                var result = await _context.TicketTransactionNotifications
                    .AsNoTrackingWithIdentityResolution()
                    .Include(x => x.AddedByUser)
                    .Include(x => x.ReceiveByUser)
                    .Where(x => x.ReceiveBy == request.UserId)
                    .AsSplitQuery()
                    .Select(x => new AllTransactionResult
                    {
                        Id = x.Id,
                        Message = x.Message,
                        Added_By = x.AddedByUser.Fullname,
                        Created_At = x.Created_At,
                        Receive_By = x.ReceiveByUser.Fullname,
                        Is_Checked = x.IsChecked,
                        Modules = x.Modules,
                        Modules_Parameter = x.Modules_Parameter,
                        PathId = x.PathId,

                    }).OrderByDescending(x => x.Created_At)
                    .ToListAsync();

                return Result.Success(result);
            }


        }


    }
}
