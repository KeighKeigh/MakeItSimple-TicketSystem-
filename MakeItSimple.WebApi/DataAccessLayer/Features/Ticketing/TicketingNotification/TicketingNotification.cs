using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.Models.Setup.BusinessUnitSetup;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.CompanySetup.GetCompany.GetCompanyResult;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketingNotification
{
    public class TicketingNotification
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
            public int TransferApprovalNotif { get; set; }
            public int ForCloseNotif { get; set; }

            public int OnHold { get; set; }

            public int NotConfirmCloseNotif { get; set; }
            public int ClosedNotif { get; set; }
            public int ForApprovalClosingNotif { get; set; }

        }

        public class BusinessUnitNotif
        {
            public int Id { get; set; }
            public bool Is_Active { get; set; }
        }

        public class TicketingNotificationCommand : IRequest<Result>
        {
            public Guid UserId { get; set; }
            public string Role { get; set; }


        }

        public class Handler : IRequestHandler<TicketingNotificationCommand, Result>
        {
            private readonly MisDbContext _context;
            private readonly IMediator _mediator;

            public Handler(MisDbContext context, IMediator mediator)
            {
                _context = context;
                _mediator = mediator;
            }

            public async Task<Result> Handle(TicketingNotificationCommand request, CancellationToken cancellationToken)
            {

                var allRequestTicketNotif = new int();
                var forTicketNotif = new int();
                var currentlyFixingNotif = new int();
                var notConfirmNotif = new int();
                var doneNotif = new int();
                var receiverForApprovalNotif = new int();
                var allTicketNotif = new int();
                var pendingTicketNotif = new int();
                var openTicketNotif = new int();
                var forTransferNotif = new int();
                var transferApprovalNotif = new int();
                var forCloseNotif = new int();
                var onHoldNotif = new int();
                var notCloseConfirmCloseNotif = new int();
                var closedNotif = new int();
                var forApprovalClosingNotif = new int();

                var allUserList = await _context.UserRoles
                    .AsNoTracking()
                    .Select(x => new
                    {
                        x.Id,
                        x.UserRoleName,
                        x.Permissions

                    }).ToListAsync();

                var requestorPermissionList = allUserList
                    .Where(x => x.Permissions
                    .Contains(TicketingConString.Requestor))
                    .Select(x => x.UserRoleName)
                    .ToList();

                var approverPermissionList = allUserList
                    .Where(x => x.Permissions
                    .Contains(TicketingConString.Approver))
                    .Select(x => x.UserRoleName)
                    .ToList();

                var receiverPermissionList = allUserList
                    .Where(x => x.Permissions
                    .Contains(TicketingConString.Receiver))
                    .Select(x => x.UserRoleName)
                    .ToList();

                var requestConcernsQuery = await _context.RequestConcerns
                    .AsNoTrackingWithIdentityResolution()
                    .Include(x => x.TicketConcerns)
                    .Where(x => x.IsActive == true)
                    .AsSplitQuery()
                    .Select(x => new
                    {
                        x.Id,
                        x.User,
                        x.UserId,
                        x.TicketConcerns,
                        x.ConcernStatus,
                        x.Is_Confirm,
                        x.IsActive,
                        x.IsDone,

                    }).ToListAsync();

                var ticketConcernQuery = await _context.TicketConcerns
                    .AsNoTrackingWithIdentityResolution()
                    .AsSplitQuery()
                    .Select(x => new
                    {
                        x.Id,
                        RequestConcern = new
                        {
                            x.RequestConcern.Is_Confirm,
                            x.RequestConcern.ConcernStatus
                        },
                        x.RequestConcernId,
                        x.User,
                        x.UserId,
                        x.IsActive,
                        x.IsDone,
                        x.IsApprove,
                        x.IsClosedApprove,
                        x.IsTransfer,
                        x.Closed_At,
                        x.OnHold,

                    }).ToListAsync();

                var transferQuery = await _context.TransferTicketConcerns
                .AsNoTrackingWithIdentityResolution()
                .Where(x => x.IsActive == true)
                .Where(x => x.IsTransfer == false)
                .AsSplitQuery()
                .Select(x => new
                {
                    x.Id,
                    TicketConcern = new
                    {
                        x.TicketConcern.OnHold,
                    },
                    x.TicketConcernId,
                    x.TicketApprover,
                    x.TransferBy,
                    x.TransferTo,

                }).ToListAsync();

                var closeQuery = await _context.ClosingTickets
                    .AsNoTrackingWithIdentityResolution()
                    .Where(x => x.IsActive)
                    .Where(x => x.IsRejectClosed == false)
                    .Where(x => x.IsClosing == false)
                    .Select(x => new
                    {
                        x.Id,
                        x.IsClosing,
                        x.TicketApprover

                    }).ToListAsync();


                if (requestorPermissionList.Any(x => x.Contains(request.Role)))
                {

                    var transferApprovalList = await _context.TransferTicketConcerns
                        .AsNoTrackingWithIdentityResolution()
                        .Where(t => t.IsTransfer == false && t.TransferTo == request.UserId)
                        .Select(t => t.TicketConcernId)
                        .ToListAsync();

                    requestConcernsQuery = requestConcernsQuery
                        .Where(x => x.UserId == request.UserId || transferApprovalList.Contains(x.Id))
                        .ToList();

                    allRequestTicketNotif = requestConcernsQuery.Count();

                    forTicketNotif = requestConcernsQuery
                            .Where(x => x.ConcernStatus == TicketingConString.ForApprovalTicket)
                            .Count();

                    currentlyFixingNotif = requestConcernsQuery
                                    .Where(x => x.ConcernStatus == TicketingConString.CurrentlyFixing)
                                    .Count();

                    notConfirmNotif = requestConcernsQuery
                        .Where(x => x.Is_Confirm == null && x.ConcernStatus == TicketingConString.NotConfirm)
                        .Count();

                    doneNotif = requestConcernsQuery
                        .Where(x => x.ConcernStatus == TicketingConString.Done && x.Is_Confirm == true)
                        .Count();


                    ticketConcernQuery = ticketConcernQuery
                        .Where(x => x.IsApprove == true && x.UserId == request.UserId && ticketConcernQuery.Any())
                        .ToList();

                    allTicketNotif = ticketConcernQuery.Count();

                    openTicketNotif = ticketConcernQuery
                         .Where(x => x.IsApprove == true && x.IsTransfer != false
                         && x.IsClosedApprove == null && x.OnHold == null)
                         .Count();

                    forTransferNotif = transferQuery
                         .Where(x => x.TransferBy == request.UserId)
                         .Count();

                    transferApprovalNotif = transferQuery
                        .Where(t => t.TransferTo == request.UserId)
                        .Count();

                    forCloseNotif = ticketConcernQuery
                        .Where(x => x.IsClosedApprove == false && x.OnHold == null)
                        .Count();

                    onHoldNotif = ticketConcernQuery
                        .Where(x => x.OnHold == true)
                        .Count();

                    notCloseConfirmCloseNotif = ticketConcernQuery
                        .Where(x => x.IsClosedApprove == true && x.RequestConcern.Is_Confirm == null && x.OnHold == null)
                        .Count();


                    closedNotif = ticketConcernQuery
                        .Where(x => x.IsClosedApprove == true && x.RequestConcern.Is_Confirm == true && x.OnHold == null)
                        .Count();
                }

                if (approverPermissionList.Any(x => x.Contains(request.Role)))
                {

                    var userApprover = await _context.Users
                        .AsNoTracking()
                        .Select(x => new
                        {
                            x.Id,
                            x.Username,

                        }).FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);

                    var approverTransactList = await _context.ApproverTicketings
                        .AsNoTrackingWithIdentityResolution()
                        .Where(x => x.UserId == userApprover.Id)
                        .Where(x => x.IsApprove == null)
                        .Select(x => new
                        {
                            x.ApproverLevel,
                            x.IsApprove,
                            x.TransferTicketConcernId,
                            x.ClosingTicketId,
                            x.UserId,

                        }).ToListAsync();


                    if (closeQuery.Any())
                    {

                        var userRequestIdApprovalList = approverTransactList
                            .Select(x => x.ClosingTicketId)
                            .ToList();

                        var userIdsInApprovalList = approverTransactList
                            .Select(approval => approval.UserId)
                            .ToList();

                        forApprovalClosingNotif = closeQuery
                              .Where(x => userIdsInApprovalList.Contains(x.TicketApprover)
                              && userRequestIdApprovalList.Contains(x.Id))
                              .Count();
                    }

                }


                if (receiverPermissionList.Any(x => x.Contains(request.Role)))
                {

                    if (requestConcernsQuery.Any())
                    {
                        var listOfRequest = requestConcernsQuery
                            .Select(x => x.User.BusinessUnitId)
                            .ToList();

                        var receiverList = await _context.Receivers
                            .AsNoTrackingWithIdentityResolution()
                            .Include(x => x.BusinessUnit)
                            .Where(x => x.IsActive == true)
                            .Where(x => listOfRequest
                            .Contains(x.BusinessUnitId))
                            .Select(x => x.BusinessUnitId)
                            .ToListAsync();

                        var receiverConcernsQuery = requestConcernsQuery
                                .Where(x => receiverList.Contains(x.User.BusinessUnitId))
                                .Select(x => x.Id)
                                .ToList();

                        receiverForApprovalNotif = await _context.TicketConcerns
                           .AsNoTrackingWithIdentityResolution()
                           .Where(x => receiverConcernsQuery.Contains(x.RequestConcernId.Value) && x.IsApprove == false)
                           .CountAsync();

                    }

                }

                var notification = new TicketingNotifResult
                {
                    AllRequestTicketNotif = allRequestTicketNotif,
                    ForTicketNotif = forTicketNotif,
                    CurrentlyFixingNotif = currentlyFixingNotif,
                    NotConfirmNotif = notConfirmNotif,
                    DoneNotif = doneNotif,
                    ReceiverForApprovalNotif = receiverForApprovalNotif,
                    AllTicketNotif = allTicketNotif,
                    OpenTicketNotif = openTicketNotif,
                    ForTransferNotif = forTransferNotif,
                    TransferApprovalNotif = transferApprovalNotif,
                    OnHold = onHoldNotif,
                    ForCloseNotif = forCloseNotif,
                    NotConfirmCloseNotif = notCloseConfirmCloseNotif,
                    ClosedNotif = closedNotif,
                    ForApprovalClosingNotif = forApprovalClosingNotif,

                };

                var confirmList = ticketConcernQuery
                    .Where(x => x.UserId == request.UserId 
                    && x.RequestConcern.Is_Confirm == null 
                    && x.RequestConcern.ConcernStatus == TicketingConString.NotConfirm)
                    .ToList();

                foreach (var confirm in confirmList)
                {

                    int hoursDifference = 24;

                    var daysClose = confirm.Closed_At.Value.Day - DateTime.Now.Day;

                    daysClose = Math.Abs(daysClose) * (1);

                    if (daysClose >= 1)
                    {
                        daysClose = daysClose * hoursDifference;
                    }

                    var hourConvert = (daysClose + confirm.Closed_At.Value.Hour) - DateTime.Now.Hour;

                    if (hourConvert >= hoursDifference)
                    {
                        var requestConcern = await _context.RequestConcerns
                            .FirstOrDefaultAsync(x => x.Id == confirm.RequestConcernId);

                        requestConcern.Is_Confirm = true;
                        requestConcern.Confirm_At = DateTime.Today;
                        requestConcern.ConcernStatus = TicketingConString.Done;

                        var ticketHistory = await _context.TicketHistories
                            .Where(x => x.TicketConcernId == confirm.Id)
                            .Where(x => x.IsApprove == null && x.Request.Contains(TicketingConString.NotConfirm))
                            .FirstOrDefaultAsync();

                        if (ticketHistory != null)
                        {
                            ticketHistory.TicketConcernId = confirm.Id;
                            ticketHistory.TransactedBy = request.UserId;
                            ticketHistory.TransactionDate = DateTime.Now;
                            ticketHistory.Request = TicketingConString.Confirm;
                            ticketHistory.Status = TicketingConString.CloseConfirm;
                        }

                        var addNewTicketTransactionNotification = new TicketTransactionNotification
                        {

                            Message = $"Ticket number {confirm.Id} has been closed",
                            AddedBy = request.UserId,
                            Created_At = DateTime.Now,
                            ReceiveBy = confirm.UserId.Value,
                            Modules = PathConString.IssueHandlerConcerns,
                            Modules_Parameter = PathConString.Closed,

                        };

                        await _context.TicketTransactionNotifications.AddAsync(addNewTicketTransactionNotification);

                    }

                }

                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success(notification);

            }
        }
    }
}
