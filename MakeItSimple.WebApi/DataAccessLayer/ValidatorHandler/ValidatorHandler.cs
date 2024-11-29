using FluentValidation;
using MakeItSimple.WebApi.Models;
using MakeItSimple.WebApi.Models.Setup.CategorySetup;
using MakeItSimple.WebApi.Models.Setup.ChannelSetup;
using MakeItSimple.WebApi.Models.Setup.SubCategorySetup;
using MakeItSimple.WebApi.Models.UserManagement.UserRoleAccount;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.CategorySetup.UpsertCategory;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.SubCategorySetup.UpsertSubCategory;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.UserManagement.UserAccount.AddNewUser;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.UserManagement.UserAccount.UserChangePassword;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.UserManagement.UserRoleAccount.AddNewUserRole;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.UserManagement.UserRoleAccount.UntagAndTagUserRolePermission;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.UserManagement.UserRoleAccount.UpdateUserRole;



namespace MakeItSimple.WebApi.DataAccessLayer.ValidatorHandler
{
    public class ValidatorHandler
    {
        //User
        public  IValidator<AddNewUserCommand> AddNewUserValidator { get; set; }
        public IValidator<UserChangePasswordCommand> UserChangePasswordValidator { get; set; }

        //UserRoles
        public IValidator<AddNewUserRoleCommand>  AddUserRoleValidator { get; set; }
        public IValidator<UpdateUserRoleCommand>  UpdateUserRoleValidator { get; set; }
        public IValidator<UntagAndTagUserRolePermissionCommand> TagAndUntagUserRoleValidator {  get; set; }

        //Category Setup
        public IValidator<UpsertCategoryCommand> UpsertCategoryValidator { get; set; }

        //Sub Category Setup 

        public IValidator<UpsertSubCategoryCommand> UpsertSubCategoryValidator { get; set; }

        public ValidatorHandler()
        {

            //User

            AddNewUserValidator = new User.UserValidator();
            UserChangePasswordValidator = new User.UserChangePasswordValidator();   

            //UserRole

            AddUserRoleValidator = new UserRole.UserRoleValidator();
            UpdateUserRoleValidator = new UserRole.UpdateUserRoleValidator();
            TagAndUntagUserRoleValidator = new UserRole.TagAndUntagUserRoleValidator();


            //Sub Category Setup

            UpsertSubCategoryValidator = new SubCategory.SubCategoryValidator();

        }
        
    }
}
