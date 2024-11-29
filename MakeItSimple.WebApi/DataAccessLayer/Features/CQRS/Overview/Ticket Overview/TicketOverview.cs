using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Overview.Ticket_Overview
{
    public partial class TicketOverview
    {

        public class Handler : IRequestHandler<TicketOverviewQuery, PagedList<TicketOverviewRecord>>
        {
            private readonly MisDbContext _context;
            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<TicketOverviewRecord>> Handle(TicketOverviewQuery request, CancellationToken cancellationToken)
            {
                var dateToday = DateTime.Today;

                var userDetails = await _context.Users.FirstOrDefaultAsync(x => x.Id == request.UserId);

                IQueryable<TicketConcern> ticketConcernQuery = _context.TicketConcerns
                   .AsNoTrackingWithIdentityResolution()
                   .Include(x => x.AddedByUser)
                    .Include(x => x.ModifiedByUser)
                    .Include(x => x.RequestorByUser)
                    .Include(x => x.User)
                    .ThenInclude(x => x.SubUnit)
                    .Include(x => x.ClosingTickets)
                    .ThenInclude(x => x.TicketAttachments)
                    .Include(x => x.TransferTicketConcerns)
                    .ThenInclude(x => x.TicketAttachments)
                    .Include(x => x.RequestConcern)
                    .ThenInclude(x => x.User)
                    .Include(x => x.RequestConcern)
                    .ThenInclude(x => x.BackJob)
                    .AsSplitQuery()
                    .OrderBy(x => x.Id);

                var allUserList = await _context.UserRoles
                    .AsNoTracking()
                    .Select(x => new
                    {
                        x.Id,
                        x.UserRoleName,
                        x.Permissions

                    })
                    .ToListAsync();

                var receiverPermissionList = allUserList
                     .Where(x => x.Permissions
                     .Contains(TicketingConString.Receiver))
                     .Select(x => x.UserRoleName)
                     .ToList();

                var approverPermissionList = allUserList
                     .Where(x => x.Permissions
                     .Contains(TicketingConString.Approver))
                     .Select(x => x.UserRoleName)
                     .ToList();

                var issueHandlerPermissionList = allUserList
                    .Where(x => x.Permissions.Contains(TicketingConString.IssueHandler))
                    .Select(x => x.UserRoleName)
                    .ToList();

                if (!string.IsNullOrEmpty(request.Search))
                    ticketConcernQuery = ticketConcernQuery
                        .Where(x => x.Id.ToString().Contains(request.Search));


                if(!string.IsNullOrEmpty(request.BackDate))
                {
                    switch (request.BackDate)
                    {

                        case TicketingConString.Today:
                            ticketConcernQuery = ticketConcernQuery
                                .Where(x => x.ticketHistories.Max(x => x.TransactionDate).Value.Equals(dateToday));
                            break;

                        case TicketingConString.Yesterday:
                            ticketConcernQuery = ticketConcernQuery
                                .Where(x => x.ticketHistories.Max(x => x.TransactionDate).Value <= SubtractDays(dateToday,1));
                            break;

                        case TicketingConString.ThreeDays:
                            ticketConcernQuery = ticketConcernQuery
                                .Where(x => x.ticketHistories.Max(x => x.TransactionDate).Value <= SubtractDays(dateToday,3));
                            break;

                        case TicketingConString.Week:
                            ticketConcernQuery = ticketConcernQuery
                                .Where(x => x.ticketHistories.Max(x => x.TransactionDate).Value <= SubtractWeeks(dateToday,1));
                            break;

                        case TicketingConString.TwoWeeks:
                            ticketConcernQuery = ticketConcernQuery
                                .Where(x => x.ticketHistories.Max(x => x.TransactionDate).Value <= SubtractWeeks(dateToday, 2));
                            break;

                        case TicketingConString.Month:
                            ticketConcernQuery = ticketConcernQuery
                                .Where(x => x.ticketHistories.Max(x => x.TransactionDate).Value <= SubtractMonth(dateToday, 1));
                            break;

                        case TicketingConString.SixMonths:
                            ticketConcernQuery = ticketConcernQuery
                                .Where(x => x.ticketHistories.Max(x => x.TransactionDate).Value <= SubtractMonth(dateToday, 6));
                            break;

                        case TicketingConString.Year:
                            ticketConcernQuery = ticketConcernQuery
                                .Where(x => x.ticketHistories.Max(x => x.TransactionDate).Value <= SubtractYear(dateToday, 1));
                            break;

                        default:
                            return new PagedList<TicketOverviewRecord>(new List<TicketOverviewRecord>(), 0, request.PageNumber, request.PageSize);
                    }

                }

                if(!string.IsNullOrEmpty(request.UserType))
                {
                    if(TicketingConString.Approver.Equals(request.UserType))
                    {
                        ticketConcernQuery = ticketConcernQuery.
                            Where(x => x.User.SubUnitId == userDetails.SubUnitId);
                    }
                    else if(TicketingConString.Receiver.Equals(request.UserType))
                    {
                        var listOfRequest = await ticketConcernQuery
                        .Select(x => x.User.BusinessUnitId)
                        .ToListAsync();

                        var businessUnitDefault = await _context.BusinessUnits
                        .AsNoTracking()
                        .Where(x => x.IsActive == true)
                        .Where(x => listOfRequest.Contains(x.Id))
                        .Select(x => x.Id)
                        .ToListAsync();

                        var receiverList = await _context.Receivers
                            .AsNoTrackingWithIdentityResolution()
                            .Include(x => x.BusinessUnit)
                            .AsSplitQuery()
                            .Where(x => businessUnitDefault.Contains(x.BusinessUnitId.Value) && x.IsActive == true &&
                             x.UserId == request.UserId)
                            .Select(x => x.BusinessUnitId)
                            .ToListAsync();

                        if (receiverPermissionList.Any(x => x.Contains(request.Role)) && receiverList.Any())
                        {

                            ticketConcernQuery = ticketConcernQuery
                                .Where(x => receiverList.Contains(x.RequestorByUser.BusinessUnitId));
                        }

                    }
                    else if(TicketingConString.IssueHandler.Equals(request.UserType))
                    {
                        ticketConcernQuery = ticketConcernQuery
                            .Where(x => x.UserId == request.UserId);
                    }
                    else if (TicketingConString.Requestor.Equals(request.UserType))
                    {
                        ticketConcernQuery = ticketConcernQuery
                                  .Where(x => x.RequestorBy == request.UserId);
                    }
                    else
                    {
                        return new PagedList<TicketOverviewRecord>(new List<TicketOverviewRecord>(), 0, request.PageNumber, request.PageSize);
                    }

                }

                var results = ticketConcernQuery
                    .Select(x => new TicketOverviewRecord
                    {

                        TicketConcernId = x.Id,
                        BackJobId = x.RequestConcern.BackJobId,
                        Request_Type = x.RequestConcern.RequestType,
                        Concerns = x.RequestConcern.Concern,
                        Requestor_Name = x.RequestorByUser.Fullname,
                        Personnel_Unit = x.User.UnitId,
                        Personnel_Id = x.UserId,
                        Personnel = x.User.Fullname,
                        Channel_Name = x.RequestConcern.Channel.ChannelName,
                        TicketCategoryDescriptions = string.Join(", ", x.RequestConcern.TicketCategories
                          .Select(x => x.Category.CategoryDescription)),
                        TicketSubCategoryDescriptions = string.Join(", ", x.RequestConcern.TicketSubCategories
                           .Select(x => x.SubCategory.SubCategoryDescription)),
                        Ticket_Status = x.IsApprove == false && x.OnHold == null ? TicketingConString.PendingRequest
                                        : x.IsApprove == true != false && x.IsClosedApprove == null ? TicketingConString.OpenTicket
                                        : x.IsClosedApprove == true && x.RequestConcern.Is_Confirm == null && x.OnHold == null ? TicketingConString.NotConfirm
                                        : x.IsClosedApprove == true && x.RequestConcern.Is_Confirm == true && x.OnHold == null ? TicketingConString.Closed
                                        : "Unknown",
                        Remarks = x.Remarks,
                        Date_Needed = x.RequestConcern.DateNeeded,
                        Notes = x.RequestConcern.Notes,
                        Contact_Number = x.RequestConcern.ContactNumber,
                        Target_Date = x.TargetDate,
                        Closed_Status = x.TargetDate.Value.Day >= x.Closed_At.Value.Day && x.IsClosedApprove == true
                        ? TicketingConString.OnTime : x.TargetDate.Value.Day < x.Closed_At.Value.Day && x.IsClosedApprove == true
                        ? TicketingConString.Delay : null,
                        Transaction_Date = x.ticketHistories.Max(x => x.TransactionDate).Value,

                    }).OrderBy(x => x.TicketConcernId);

                return PagedList<TicketOverviewRecord>.Create(results, request.PageNumber, request.PageSize);
            }

            private static DateTime SubtractDays(DateTime date, int days)
            {
                return date.AddDays(-1 * (- days));
            }


            private static DateTime SubtractWeeks(DateTime date, int weeks)
            {
                return date.AddDays(-7 * (- weeks));
            }

            private static DateTime SubtractMonth(DateTime date, int month)
            {
                return date.AddMonths(-1 * (-month));
            }

            private static DateTime SubtractYear(DateTime date, int years)
            {
                return date.AddYears(-1 * (-years));
            }

        }
    }
}
