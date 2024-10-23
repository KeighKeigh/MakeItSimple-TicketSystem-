﻿using MakeItSimple.WebApi.Common;
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

            public int OnHold {  get; set; }    

            public int NotConfirmCloseNotif { get; set; }
            public int ClosedNotif { get; set; }
            public int ForApprovalTransferNotif { get; set; }
            public int ForApprovalClosingNotif { get; set; }

        }

        public class BusinessUnitNotif
        {
            public int Id { get; set; }
            public bool Is_Active {  get; set; }
        }

        public class TicketingNotificationCommand : IRequest<Result>
        {
            public Guid UserId { get; set; }
            public string Role {  get; set; }


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

                var allRequestTicketNotif = new List<int>();
                var forTicketNotif = new List<int>();
                var currentlyFixingNotif = new List<int>();
                var notConfirmNotif = new List<int>();
                var doneNotif = new List<int>();
                var receiverForApprovalNotif = new List<int?>();
                var allTicketNotif = new List<int>();
                var pendingTicketNotif = new List<int>();
                var openTicketNotif = new List<int>();
                var forTransferNotif = new List<int>();
                var transferApprovalNotif = new List<int>();
                var forCloseNotif = new List<int>();
                var onHoldNotif = new List<int>();
                var notCloseConfirmCloseNotif = new List<int>();
                var closedNotif = new List<int>();
                var forApprovalClosingNotif = new List<int>();

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
                    .AsNoTracking()
                    .Include(x => x.TicketConcerns)
                    .Include(x => x.User)
                    .Where(x => x.IsActive == true)
                    .Select(x => new
                    {
                        x.Id,
                        x.TicketConcerns,
                        x.User,
                        x.UserId,
                        x.ConcernStatus,
                        x.Is_Confirm,                 
                        x.IsActive,
                        x.IsDone,

                    }).ToListAsync();

                var ticketConcernQuery = await _context.TicketConcerns
                    .AsNoTracking()
                    .Include(x => x.RequestConcern)
                    .Include(x => x.User)
                    .Include(x => x.RequestorByUser)
                    .Select(x => new
                    {
                        x.Id,
                        x.RequestConcern,
                        x.RequestConcernId,
                        x.User,
                        x.UserId,
                        x.RequestorByUser,
                        x.IsActive,
                        x.IsDone,
                        x.IsApprove,
                        x.IsClosedApprove,
                        x.IsTransfer,
                        x.Closed_At,
                        x.OnHold,
                      
                    }).ToListAsync();

                var transferQuery = await _context.TransferTicketConcerns
                .AsNoTracking()
                .Include(x => x.TicketConcern)
                .ThenInclude(x => x.User)
                .Where(x => x.IsActive == true)
                .Where(x => x.IsRejectTransfer == false)
                .Select(x => new
                {
                    x.Id,
                    x.TicketConcern,
                    x.TicketConcernId,
                    x.TicketApprover,
                    x.IsTransfer,
                    x.TransferBy,
                    x.TransferTo,
            


                }).ToListAsync();

                var closeQuery = await _context.ClosingTickets
                    .AsNoTracking()
                    .Include(x => x.TicketConcern)
                    .ThenInclude(x => x.User)
                    .Where(x => x.IsActive)
                    .Where(x => x.IsRejectClosed == false)
                    .Where(x => x.IsClosing == false)
                    .Select(x => new
                    {
                        x.Id,
                        x.TicketConcern,
                        x.IsClosing,
                        x.TicketApprover

                    }).ToListAsync();


                if (requestorPermissionList.Any(x => x.Contains(request.Role)))
                {

                    var transferApprovalList = _context.TransferTicketConcerns
                        .Where(t => t.IsTransfer == false && t.TransferTo == request.UserId)
                        .Select(t => t.TicketConcernId);

                    requestConcernsQuery = requestConcernsQuery
                        .Where(x => x.UserId == request.UserId || transferApprovalList.Contains(x.Id) && requestConcernsQuery.Any())
                        .ToList();

                    var allRequestTicket = requestConcernsQuery
                        .Select(x => x.Id)
                        .ToList();


                        allRequestTicketNotif = allRequestTicket;

                    var forApprovalTicket = requestConcernsQuery
                            .Where(x => x.ConcernStatus == TicketingConString.ForApprovalTicket)
                            .Select(x => x.Id)
                            .ToList();


                    forTicketNotif = forApprovalTicket;


                    var currentlyFixing = requestConcernsQuery
                                    .Where(x => x.ConcernStatus == TicketingConString.CurrentlyFixing)
                                    .Select(x => x.Id)
                                    .ToList();

                    currentlyFixingNotif = currentlyFixing;

                    var notConfirm = requestConcernsQuery
                        .Where(x => x.Is_Confirm == null && x.ConcernStatus == TicketingConString.NotConfirm)
                        .Select (x => x.Id)
                        .ToList();


                    notConfirmNotif = notConfirm;


                    var done = requestConcernsQuery
                        .Where(x => x.ConcernStatus == TicketingConString.Done && x.Is_Confirm == true)
                        .Select (x => x.Id)
                        .ToList();

                    doneNotif = done;


                    ticketConcernQuery = ticketConcernQuery
                        .Where(x => x.IsApprove == true && x.UserId == request.UserId && ticketConcernQuery.Any())
                        .ToList();

                    var allTicketConcern = ticketConcernQuery
                        .Select(x => x.Id)
                        .ToList();


                    allTicketNotif = allTicketConcern;

                    var openTicket = ticketConcernQuery
                         .Where(x => x.IsApprove == true && x.IsTransfer != false
                         &&  x.IsClosedApprove == null && x.OnHold == null)
                         .Select(x => x.Id)
                         .ToList();

                    openTicketNotif = openTicket;

                    var forTransferTicket = transferQuery
                         .Where(x => x.IsTransfer == false && x.TransferBy == request.UserId)
                         .Select (x => x.Id)
                         .ToList();


                    forTransferNotif = forTransferTicket;


                    var transferApproval = transferQuery
                        .Where(t => t.IsTransfer == false && t.TicketConcern.OnHold == null && t.TransferTo == request.UserId)
                        .Select(x => x.Id)
                        .ToList();

                    transferApprovalNotif = transferApproval;

                    var forClosedTicket = ticketConcernQuery
                        .Where(x => x.IsClosedApprove == false && x.OnHold == null)
                        .Select(x => x.Id)  
                        .ToList();


                    forCloseNotif = forClosedTicket;

                    var onHold = ticketConcernQuery
                        .Where(x => x.OnHold == true)
                        .Select(x => x.Id)
                        .ToList ();

                    onHoldNotif = onHold;

                    var notConfirmTicket = ticketConcernQuery
                        .Where(x => x.IsClosedApprove == true && x.RequestConcern.Is_Confirm == null && x.OnHold == null)
                        .Select(x => x.Id)  
                        .ToList();


                    notCloseConfirmCloseNotif = notConfirmTicket;



                    var closedTicket = ticketConcernQuery 
                        .Where(x => x.IsClosedApprove == true && x.RequestConcern.Is_Confirm == true && x.OnHold == null)
                        .Select(x => x.Id)
                        .ToList();


                    closedNotif = closedTicket;
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


                    if(closeQuery.Any())
                    {
                         
                        var userRequestIdApprovalList = approverTransactList
                            .Select(x => x.ClosingTicketId)
                            .ToList();

                        var userIdsInApprovalList = approverTransactList
                            .Select(approval => approval.UserId)
                            .ToList();

                      var closeForApproval = closeQuery
                            .Where(x => userIdsInApprovalList.Contains(x.TicketApprover)
                            && userRequestIdApprovalList.Contains(x.Id))
                            .Select(x => x.Id)
                            .ToList();

                        forApprovalClosingNotif = closeForApproval;
                        
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

                        var forApprovalConcerns = await _context.TicketConcerns
                            .AsNoTrackingWithIdentityResolution()
                            .Where(x => receiverConcernsQuery.Contains(x.RequestConcernId.Value) && x.IsApprove == false)
                            .Select(x => x.RequestConcernId)
                            .ToListAsync();

                        receiverForApprovalNotif = forApprovalConcerns;

                    }

                }


                var notification = new TicketingNotifResult
                {
                    AllRequestTicketNotif = allRequestTicketNotif.Count(),
                    ForTicketNotif = forTicketNotif.Count(),
                    CurrentlyFixingNotif = currentlyFixingNotif.Count(),
                    NotConfirmNotif = notConfirmNotif.Count(),
                    DoneNotif = doneNotif.Count(),
                    ReceiverForApprovalNotif = receiverForApprovalNotif.Count(),
                    AllTicketNotif = allTicketNotif.Count(),
                    OpenTicketNotif = openTicketNotif.Count(),
                    ForTransferNotif = forTransferNotif.Count(),
                    TransferApprovalNotif = transferApprovalNotif.Count(),
                    ForCloseNotif = forCloseNotif.Count(),
                    NotConfirmCloseNotif = notCloseConfirmCloseNotif.Count(),
                    ClosedNotif = closedNotif.Count(),
                    ForApprovalClosingNotif = forApprovalClosingNotif.Count(),

                };

                var confirmList  = ticketConcernQuery
                    .Where(x => x.UserId == request.UserId)
                    .Where(x => x.RequestConcern.ConcernStatus == TicketingConString.NotConfirm
                    && x.RequestConcern.Is_Confirm == null)
                    .ToList();

                foreach (var confirm  in confirmList)
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
