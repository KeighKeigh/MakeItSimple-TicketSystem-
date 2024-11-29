using MakeItSimple.WebApi.DataAccessLayer.Errors;
using MakeItSimple.WebApi.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MakeItSimple.WebApi.Models;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.Common.ConstantString;
using System.Data;
using Dapper;
using CloudinaryDotNet;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.UserManagement.UserAccount
{
    public class GetUser
    {

        public class GetUserResult
        {
            public Guid Id { get; set; }
            public string EmpId { get; set; }
            public string Fullname { get; set; }
            public string Username { get; set; }
            public string Added_By { get; set; }
            public DateTime Created_At { get; set; }
            public bool Is_Active { get; set; }
            public string Modified_By { get; set; }
            public DateTime? Update_At { get; set; }

            public string Profile_Pic { get; set; }
            public string FileName { get; set; }
            public decimal? FileSize { get; set; }

            public int? UserRoleId { get; set; }
            public string User_Role_Name { get; set; }

            public int? DepartmentId { get; set; }
            public string Department_Code { get; set; }
            public string Department_Name { get; set; }

            public int? CompanyId { get; set; }
            public string Company_Code { get; set; }
            public string Company_Name { get; set; }

            public int? LocationId { get; set; }
            public string Location_Code { get; set; }
            public string Location_Name { get; set; }

            public int? BusinessUnitId { get; set; }
            public string BusinessUnit_Code { get; set; }
            public string BusinessUnit_Name { get; set; }

            public int? UnitId { get; set; }
            public string Unit_Code { get; set; }
            public string Unit_Name { get; set; }

            public int? SubUnitId { get; set; }
            public string SubUnit_Code { get; set; }
            public string SubUnit_Name { get; set; }
            public ICollection<string> Permission { get; set; }

            //public string PermissionJson { get; set; }
            //public List<string>  Permission 
            //{
            //    get
            //    {
            //        if (!string.IsNullOrWhiteSpace(PermissionJson))
            //        {
            //            return JsonConvert.DeserializeObject<List<string>>(PermissionJson);
            //        }
            //        return new List<string>();
            //    }
            //}

            public bool Is_Use { get; set; }

            public bool? Is_Store { get; set; }


        }

        public class GetUsersQuery : UserParams, IRequest<PagedList<GetUserResult>>
        {
            public bool? Status { get; set; }
            public string Search { get; set; }
        }

        public class Handler : IRequestHandler<GetUsersQuery, PagedList<GetUserResult>>
        {

            private readonly MisDbContext _context;
            private readonly IDbConnection _dbConnection;

            public Handler(MisDbContext context, IDbConnection dbConnection)
            {
                _context = context;
                _dbConnection = dbConnection;
            }

            public async Task<PagedList<GetUserResult>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
            {

                if (request == null)
                {
                    //                var sql = @"
                    //                    SELECT 
                    //                        u.Id As Id ,
                    //                        u.Emp_Id As EmpId,
                    //                        u.Fullname AS Fullname,
                    //                        u.Username As Username,
                    //                        u.Created_At,
                    //                        u.Is_Active,
                    //                        u.Profile_Pic,
                    //                        u.file_name AS FileName,
                    //                        u.file_size AS FileSize,
                    //                        u.updated_at,
                    //                        ur.Id AS UserRoleId,
                    //                        ur.User_Role_Name,
                    //                        d.Id AS DepartmentId,
                    //                        d.Department_Code,
                    //                        d.Department_Name,
                    //                        c.Id AS CompanyId,
                    //                        c.Company_Code,
                    //                        c.Company_Name,
                    //                        l.Id AS LocationId,
                    //                        l.Location_Code,
                    //                        l.Location_Name,
                    //                        bu.Id AS BusinessUnitId,
                    //                        bu.Business_Code As businessUnit_Code,
                    //                        bu.Business_Name As businessUnit_Name,
                    //                        un.Id AS UnitId,
                    //                        un.Unit_Code,
                    //                        un.Unit_Name,
                    //                        su.Id AS SubUnitId,
                    //                        su.Sub_Unit_Code As SubUnit_Code,
                    //                        su.Sub_Unit_Name As SubUnit_Name,
                    //                        ur.permissions As PermissionJson,
                    //                        Case 
                    //                        WHEN (SELECT COUNT(*) FROM Approvers a WHERE a.User_Id = u.Id) > 0 OR 
                    //                             (SELECT COUNT(*) FROM Receivers r WHERE r.User_Id = u.Id) > 0 OR
                    //                             (SELECT COUNT(*) FROM approver_ticketings at WHERE at.User_Id = u.Id AND at.Is_Approve IS NULL) > 0 OR
                    //                             (ur.user_role_name LIKE '%'+ @IssueHandler + '%' AND tc.is_approve = 1 AND tc.is_closed_approve IS NOT NULL)
                    //                        THEN 1
                    //                        ELSE 0
                    //                        END AS Is_Use,
                    //                        u.Is_Store
                    //                    FROM Users u
                    //                    LEFT JOIN User_Roles ur ON u.User_Role_Id = ur.Id
                    //                    LEFT JOIN Departments d ON u.Department_Id = d.Id
                    //                    LEFT JOIN Companies c ON u.Company_Id = c.Id
                    //                    LEFT JOIN Locations l ON u.Location_Id = l.Id
                    //                    LEFT JOIN Business_Units bu ON u.Business_Unit_Id = bu.Id
                    //                    LEFT JOIN Units un ON u.Unit_Id = un.Id
                    //                    LEFT JOIN Sub_Units su ON u.Sub_Unit_Id = su.Id
                    //                    LEFT JOIN Ticket_Concerns tc ON u.id = tc.user_id

                    //";

                    //                var results =  await _dbConnection.QueryAsync<GetUserResult>(sql, new
                    //                {
                    //                    Search = request.Search,
                    //                    Status = request.Status,
                    //                    IssueHandler = TicketingConString.IssueHandler

                    //                });


                    //                return  PagedList<GetUserResult>.Create(results.AsQueryable(), request.PageNumber, request.PageSize);

                }

                IQueryable<User> userQuery = _context.Users
                    .Include(x => x.AddedByUser)
                    .Include(x => x.ModifiedByUser)
                    .Include(x => x.UserRole);

                if (!string.IsNullOrEmpty(request.Search))
                {
                    userQuery = userQuery.Where(x => x.Fullname.Contains(request.Search)
                    || x.UserRole.UserRoleName.Contains(request.Search));
                }

                if (request.Status != null)
                {
                    userQuery = userQuery.Where(x => x.IsActive == request.Status);

                }


                var userPermissions = new List<string>();

                var users = userQuery.Select(x => new GetUserResult
                {

                    Id = x.Id,
                    EmpId = x.EmpId,
                    Fullname = x.Fullname,
                    Username = x.Username,
                    Added_By = x.AddedByUser.Fullname,
                    Created_At = x.CreatedAt,
                    Is_Active = x.IsActive,
                    Modified_By = x.ModifiedByUser.Fullname,
                    Profile_Pic = x.ProfilePic,
                    FileName = x.FileName,
                    FileSize = x.FileSize,
                    Update_At = x.UpdatedAt,
                    UserRoleId = x.UserRoleId,
                    User_Role_Name = x.UserRole.UserRoleName,
                    DepartmentId = x.DepartmentId,
                    Department_Code = x.Department.DepartmentCode,
                    Department_Name = x.Department.DepartmentName,
                    SubUnitId = x.SubUnitId,
                    SubUnit_Code = x.SubUnit.SubUnitCode,
                    SubUnit_Name = x.SubUnit.SubUnitName,
                    CompanyId = x.CompanyId,
                    Company_Code = x.Company.CompanyCode,
                    Company_Name = x.Company.CompanyName,
                    LocationId = x.LocationId,
                    Location_Code = x.Location.LocationCode,
                    Location_Name = x.Location.LocationName,
                    BusinessUnitId = x.BusinessUnitId,
                    BusinessUnit_Code = x.BusinessUnit.BusinessCode,
                    BusinessUnit_Name = x.BusinessUnit.BusinessName,
                    UnitId = x.UnitId,
                    Unit_Code = x.Units.UnitCode,
                    Unit_Name = x.Units.UnitName,
                    Permission = x.UserRole.Permissions != null ? x.UserRole.Permissions : userPermissions,
                    Is_Use = x.Approvers.Any() || x.Receivers.Any() ||
                    x.ApproversTickets.Any(x => x.IsApprove == null) ||
                    x.UserRole.UserRoleName.Contains(TicketingConString.IssueHandler)
                    && x.TicketConcerns.Any(x => x.IsApprove == true && x.IsClosedApprove == null) ?
                      true : false,
                    Is_Store = x.IsStore,

                });

                return await PagedList<GetUserResult>.CreateAsync(users, request.PageNumber, request.PageSize);


            }


        }



    }
}
