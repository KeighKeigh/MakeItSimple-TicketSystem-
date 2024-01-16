﻿using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup;
using MakeItSimple.WebApi.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.UserFeatures
{
    public class UpdateUser
    {

        public class UpdateUserResult
        {
            public Guid Id {  get; set; }
            public string Email { get; set; }
            public int UserRoleId { get; set; }

            public int ? DepartmentId { get; set; }

            public int ? SubUnitId {  get; set; }

            public Guid ? Modified_By { get; set; }
            public DateTime ? Updated_At { get; set; }

        }

        public class UpdateUserCommand : IRequest<Result>
        {
            public Guid Id { get; set; }
            public string Email { get; set; }
            public int UserRoleId { get; set; }
            public int? DepartmentId { get; set; }

            public int ? SubUnitId { get; set; }

            public Guid? Modified_By { get; set; }
        }

        public class Handler : IRequestHandler<UpdateUserCommand, Result>
        {
            private readonly MisDbContext _context;
            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
            {

                var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == command.Id , cancellationToken);

                if (user == null)
                {
                    return Result.Failure(UserError.UserNotExist());

                }
                else if (user.Email == command.Email && user.UserRoleId == command.UserRoleId
                    && user.DepartmentId == command.DepartmentId && user.SubUnitId == command.SubUnitId)
                {
                    return Result.Failure(UserError.UserNoChanges());
                }


                var EmailAlreadyExist = await _context.Users.FirstOrDefaultAsync(x => x.Email == command.Email, cancellationToken);

                if (EmailAlreadyExist != null && user.Email != command.Email)
                {
                    return Result.Failure(UserError.EmailAlreadyExist(command.Email));
                }

                var userRoleNotExist = await _context.UserRoles.FirstOrDefaultAsync(x => x.Id == command.UserRoleId , cancellationToken);

                if (userRoleNotExist == null)
                {
                    return Result.Failure(UserError.UserRoleNotExist());
                }

                var departmentNotExist = await _context.Departments.FirstOrDefaultAsync(x => x.Id == command.DepartmentId, cancellationToken);

                if (departmentNotExist == null)
                {
                    return Result.Failure(UserError.DepartmentNotExist());
                }

                var subUnitNotExist = await _context.SubUnits.FirstOrDefaultAsync(x => x.Id == command.SubUnitId, cancellationToken);

                if (subUnitNotExist == null)
                {
                    return Result.Failure(UserError.SubUnitNotExist());
                }

                var userIsUse = await _context.ChannelUsers.AnyAsync(x => x.UserId == command.Id, cancellationToken);
                if (userIsUse == true)
                {
                    return Result.Failure(UserError.UserIsUse(user.Fullname));
                }

                user.Email = command.Email;
                user.UserRoleId = command.UserRoleId;
                user.DepartmentId = command.DepartmentId;
                user.SubUnitId = command.SubUnitId;
                user.UpdatedAt = DateTime.Now;
                user.ModifiedBy = command.Modified_By;

                await _context.SaveChangesAsync(cancellationToken);

                var result = new UpdateUserResult
                {
                    Id = command.Id,    
                    Email = user.Email,
                    UserRoleId = user.UserRoleId,
                    DepartmentId = user.DepartmentId,
                    SubUnitId = user.SubUnitId,
                    Updated_At = user.UpdatedAt,
                    Modified_By = user.ModifiedBy,
                };

                return Result.Success(result);




            }
        }



    }
}