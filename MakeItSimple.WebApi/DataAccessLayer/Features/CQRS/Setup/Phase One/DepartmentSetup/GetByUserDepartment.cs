using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.DepartmentSetup.GetUserDepartment;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.DepartmentSetup
{
    public class GetByUserDepartment
    {
        public record GetByUserDepartmentResult
        {
            public int departmentId { get; set; }
            public Guid? UserId { get; set; }
            public string Fullname { get; set; }

            public string Role { get; set; }

        }

        public class GetUserByDepartmentCommand : IRequest<Result>
        {
            public Guid? UserId { get; set; }
        }

        public class Handler : IRequestHandler<GetUserByDepartmentCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(GetUserByDepartmentCommand request, CancellationToken cancellationToken)
            {

                var result = await _context.Users
                    .Include(x => x.UserRole)
                    .Where(x => x.IsActive == true && x.Id == request.UserId)
                    .Select(x => new GetByUserDepartmentResult
                    {
                        departmentId = x.DepartmentId.Value,
                        UserId = x.Id,
                        Fullname = x.Fullname,
                        Role = x.UserRole.UserRoleName,

                    }).ToListAsync();

                return Result.Success(result);
            }
        }
    }
}
