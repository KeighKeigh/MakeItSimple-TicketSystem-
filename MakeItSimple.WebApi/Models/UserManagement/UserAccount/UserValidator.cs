﻿using FluentValidation;
using static MakeItSimple.WebApi.DataAccessLayer.Features.UserFeatures.AddNewUser;
using static MakeItSimple.WebApi.DataAccessLayer.Features.UserFeatures.UpdateUser;


namespace MakeItSimple.WebApi.Models
{
    public partial class User 
    {
        public class UserValidator : AbstractValidator<AddNewUserCommand>
        {
            public UserValidator()
            {

                
                RuleFor(f => f.Fullname).NotEmpty().WithMessage("Fullname is required!")
                .MinimumLength(3).WithMessage("Fullname must be at least 3 character long!");
                RuleFor(u => u.Username).NotEmpty().WithMessage("Username is required!")
                .MinimumLength(3).WithMessage("Username must be at least 3 character long!");
                RuleFor(em => em.Email).NotEmpty().WithMessage("Email is required!")
                .EmailAddress().WithMessage("A valid email address is required!");
               
            }

        }


        public class UserUpdateValidator : AbstractValidator<UpdateUserCommand>
        {
            public UserUpdateValidator()
            {

                RuleFor(em => em.Email).NotEmpty().WithMessage("Email is required!")
                .EmailAddress().WithMessage("A valid email address is required!");

            }

        }







    }
}
