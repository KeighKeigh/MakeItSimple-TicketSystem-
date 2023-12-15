﻿using MakeItSimple.WebApi.Common;


namespace MakeItSimple.WebApi.DataAccessLayer.Errors
{
    public class UserError
    {
      public static Error NoDataFound() =>
      new Error("User.NoDataFound", "No Data found");

      public static Error UserAlreadyExist(string Fullname) =>
      new Error("User.UserAlreadyExist", $"User {Fullname} already Exist");

      public static Error UserRoleNotExist() =>
      new Error("User.UserRoleNotExist", "User role not exist!");
      
      public static Error UserNotExist() =>
      new Error("User.UserNotExist", "User not exist!");

    }
}
