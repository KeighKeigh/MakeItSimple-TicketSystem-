using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosedTicketConcern.GetClosing.GetClosingTicket;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket.GetTransfer.GetTransferTicket;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket.GetTransfer.GetTransferTicket.GetTransferTicketResult;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.OnHoldTicket.Get_OnHold
{
    public partial class GetOnHold
    {

        public class Handler : IRequestHandler<GetOnHoldQuery, PagedList<GetOnHoldReports>>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<GetOnHoldReports>> Handle(GetOnHoldQuery request, CancellationToken cancellationToken)
            {
                IQueryable<TicketOnHold> onHoldQuery = _context.TicketOnHolds
                    .AsNoTrackingWithIdentityResolution()
                    .Include(x => x.AddedByUser)
                    .Include(x => x.TicketConcern)
                    .ThenInclude(x => x.RequestConcern)
                    .ThenInclude(x => x.TicketCategories)
                    .Include(x => x.TicketConcern)
                    .ThenInclude(x => x.RequestConcern)
                    .ThenInclude(x => x.TicketSubCategories)
                    .Include(x => x.TicketAttachments)
                    .Include(x => x.ApproverTickets)
                    .Include(x => x.RejectOnHoldByUser)
                    .AsSplitQuery();

                if(onHoldQuery.Any())
                {
                    var allUserList = await _context.UserRoles
                        .AsNoTracking()
                        .Select(x => new
                        {
                            x.Id,
                            x.UserRoleName,
                            x.Permissions

                        }).ToListAsync();

                    var approverPermissionList = allUserList
                        .Where(x => x.Permissions
                        .Contains(TicketingConString.Approver))
                        .Select(x => x.UserRoleName)
                        .ToList();


                    if (!string.IsNullOrEmpty(request.Search))
                    {
                        onHoldQuery = onHoldQuery
                            .Where(x => x.TicketConcern.User.Fullname
                            .Contains(request.Search)
                        || x.TicketConcernId.ToString().Contains(request.Search));
                    }


                    if (request.IsHold is not null)
                    {
                        onHoldQuery = onHoldQuery.Where(x => x.IsHold == request.IsHold);
                    }

                    if (request.IsReject is not null)
                    {
                        onHoldQuery = onHoldQuery.Where(x => x.IsRejectOnHold == request.IsReject);
                    }

                    if (!string.IsNullOrEmpty(request.UserType))
                    {
                        var filterApproval = onHoldQuery.Select(x => x.Id);

                        if (request.UserType.Equals(TicketingConString.Approver))
                        {
                            if (approverPermissionList.Any(x => x.Contains(request.Role)))
                            {
                                var userApprover = await _context.Users
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);

                                var approverTransactList = await _context.ApproverTicketings
                                    .AsNoTracking()
                                    .Where(x => x.UserId == userApprover.Id)
                                    .Where(x => x.IsApprove == null)
                                    .Select(x => new
                                    {
                                        x.ApproverLevel,
                                        x.IsApprove,
                                        x.TicketOnHoldId,
                                        x.UserId,

                                    }).ToListAsync();

                                var userRequestIdApprovalList = approverTransactList.Select(x => x.TicketOnHoldId);
                                var userIdsInApprovalList = approverTransactList.Select(approval => approval.UserId);

                                onHoldQuery = onHoldQuery
                                    .Where(x => userIdsInApprovalList.Contains(x.TicketApprover)
                                    && userRequestIdApprovalList.Contains(x.Id));

                            }
                            else
                            {
                                return new PagedList<GetOnHoldReports>(new List<GetOnHoldReports>(), 0, request.PageNumber, request.PageSize);
                            }

                        }
                        else if (request.UserType.Equals(TicketingConString.IssueHandler))
                        {
                            onHoldQuery = onHoldQuery.Where(x => x.AddedByUser.Id == request.UserId);
                        }
                        else
                        {
                            return new PagedList<GetOnHoldReports>(new List<GetOnHoldReports>(), 0, request.PageNumber, request.PageSize);
                        }
                    }
                }

                var results = onHoldQuery
                    .Where(x => x.IsActive)
                   .OrderBy(x => x.Id)
                   .Select(x => new GetOnHoldReports
                   {
                       TicketConcernId = x.TicketConcernId,
                       TicketOnHoldId = x.Id,
                       ChannelId = x.TicketConcern.RequestConcern.ChannelId,
                       Channel_Name = x.TicketConcern.RequestConcern.Channel.ChannelName,
                       UserId = x.TicketConcern.UserId,
                       
                       Fullname = x.TicketConcern.User.Fullname,
                       Concern_Details = x.TicketConcern.RequestConcern.Concern,
                       Reason = x.Reason,
                       GetOnHoldTicketCategories = x.TicketConcern.RequestConcern.TicketCategories
                                               .Select(tc => new GetOnHoldReports.GetOnHoldTicketCategory
                                               {
                                                   TicketCategoryId = tc.Id,
                                                   CategoryId = tc.CategoryId,
                                                   Category_Description = tc.Category.CategoryDescription,

                                               }).ToList(),
                       GetOnHoldTicketSubCategories = x.TicketConcern.RequestConcern.TicketSubCategories
                        .Select(tc => new GetOnHoldReports.GetOnHoldTicketSubCategory
                        {
                            TicketSubCategoryId = tc.Id,
                            SubCategoryId = tc.SubCategoryId,
                            SubCategory_Description = tc.SubCategory.SubCategoryDescription,

                        }).ToList(),
                       Resume_At = x.ResumeAt.Value.Date,
                       OnHold_Remarks = x.OnHoldRemarks,
                       IsReject_OnHold = x.IsRejectOnHold,
                       Reject_Remarks = x.RejectRemarks,
                       OnHold_Status = x.IsHold == false && x.IsRejectOnHold == false ? "For On-Hold Approval"
                                :x.IsHold == true && x.IsRejectOnHold == false ? "On-Hold Approved"
                                :x.IsRejectOnHold == true ? "On-Hold Rejected" :"Unknown",
                       OnHoldAttachments = x.TicketAttachments
                      .Select(x => new GetOnHoldReports.OnHoldAttachment
                      {
                          TicketAttachmentId = x.Id,
                          Attachment = x.Attachment,
                          FileName = x.FileName,
                          FileSize = x.FileSize,
                          Added_By = x.AddedByUser.Fullname,
                          Created_At = x.CreatedAt,
                          Modified_By = x.ModifiedByUser.Fullname,
                          Updated_At = x.UpdatedAt
                      }).ToList(),

                   });

                return await PagedList<GetOnHoldReports>.CreateAsync(results, request.PageNumber, request.PageSize);
            }
        }
    }
}
