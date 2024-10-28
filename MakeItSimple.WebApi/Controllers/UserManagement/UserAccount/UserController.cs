﻿using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Caching;
using MakeItSimple.WebApi.Common.Extension;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Features.UserManagement.UserAccount;
using MakeItSimple.WebApi.DataAccessLayer.ValidatorHandler;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static MakeItSimple.WebApi.DataAccessLayer.Feature.UserFeatures.GetUser;
using static MakeItSimple.WebApi.DataAccessLayer.Features.UserFeatures.AddNewUser;
using static MakeItSimple.WebApi.DataAccessLayer.Features.UserFeatures.UpdateUser;
using static MakeItSimple.WebApi.DataAccessLayer.Features.UserManagement.UserAccount.UpdateProfilePic;
using static MakeItSimple.WebApi.DataAccessLayer.Features.UserManagement.UserAccount.UpdateUserStatus;
using static MakeItSimple.WebApi.DataAccessLayer.Features.UserManagement.UserAccount.UserChangePassword;
using static MakeItSimple.WebApi.DataAccessLayer.Features.UserManagement.UserAccount.UserResetPassword;
using static NuGet.Packaging.PackagingConstants;


namespace MakeItSimple.WebApi.Controllers.UserController
{
    [Route("api/User")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly IMediator _mediator;
        private readonly ValidatorHandler _validatorHandler;
        private readonly ICacheService _cacheService;


        public UserController(IMediator mediator, ValidatorHandler validatorHandler, ICacheService cacheService)
        {
            _mediator = mediator;
            _validatorHandler = validatorHandler;
            _cacheService = cacheService;
        }

        //[HttpGet("GetUser")]
        //public async Task<IActionResult> GetUser([FromQuery] GetUsersQuery query)
        //{
        //    try
        //    {
        //        var cacheKey = $"users_{query.PageNumber}_{query.PageSize}_{query.Search}_{query.Status}";

        //        var users = await _cacheService.GetOrSetAsync<PagedList<GetUserResult>>(
        //            cacheKey,
        //            () => _mediator.Send(query),
        //            TimeSpan.FromMinutes(5)
        //        );

        //        if (users == null)
        //        {
        //            return NotFound();
        //        }

        //        var successResult = Result.Success(users);

        //        Response.AddPaginationHeader(
        //            users.CurrentPage,
        //            users.PageSize,
        //            users.TotalCount,
        //            users.TotalPages,
        //            users.HasPreviousPage,
        //            users.HasNextPage
        //        );

        //        return Ok(successResult);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Conflict(ex.Message);
        //    }
        //}


        [HttpGet("GetUser")]
        public async Task<IActionResult> GetUser([FromQuery] GetUsersQuery query)
        {
            try
            {
                var users = await _mediator.Send(query);

                Response.AddPaginationHeader(

                users.CurrentPage,
                users.PageSize,
                users.TotalCount,
                users.TotalPages,
                users.HasPreviousPage,
                users.HasNextPage

                );

                var result = new
                {
                    users,
                    users.CurrentPage,
                    users.PageSize,
                    users.TotalCount,
                    users.TotalPages,
                    users.HasPreviousPage,
                    users.HasNextPage
                };

                var successResult = Result.Success(result);
                return Ok(successResult);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }


        [HttpPost("AddNewUser")]
        public async Task<IActionResult> AddNewUser([FromBody] AddNewUserCommand command)
        {
            try
            {
                var validationResult = await _validatorHandler.AddNewUserValidator.ValidateAsync(command);

                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors);
                }

                if (User.Identity is ClaimsIdentity identity && Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                {
                    command.Added_By = userId;
                }

                var result = await _mediator.Send(command);
                if (result.IsFailure)
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }

        }

        [HttpPut("UpdateUser")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserCommand command)
        {
            try
            {

                if (User.Identity is ClaimsIdentity identity && Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                {
                    command.Modified_By = userId;
                }

                var result = await _mediator.Send(command);
                if (result.IsFailure)
                {
                    return BadRequest(result);
                }
                return Ok(result);

            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }


        [HttpPatch("UpdateUserStatus")]
        public async Task<IActionResult> UpdateUserStatus([FromBody] UpdateUserStatusCommand command)
        {
            try
            {

                var result = await _mediator.Send(command);
                if (result.IsFailure)
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }


        [HttpPut("UserChangePassword")]
        public async Task<IActionResult> UserChangePassword([FromBody] UserChangePasswordCommand command)
        {
            try
            {

                var validationResult = await _validatorHandler.UserChangePasswordValidator.ValidateAsync(command);
                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors);

                }

                var result = await _mediator.Send(command);
                if (result.IsFailure)
                {
                    return BadRequest(result);
                }
                return Ok(result);

            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }


        [HttpPut("UserResetPassword")]
        public async Task<IActionResult> UserResetPassword([FromBody] UserResetPasswordCommand command)
        {
            try
            {

                var result = await _mediator.Send(command);
                if (result.IsFailure)
                {
                    return BadRequest(result);
                }
                return Ok(result);

            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateProfilePic([FromForm] UpdateProfilePicCommand command)
        {
            try
            {

                if (User.Identity is ClaimsIdentity identity && Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                {
                    command.UserId = userId;
                }

                var result = await _mediator.Send(command);
                if (result.IsFailure)
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }

        }



    }
}
