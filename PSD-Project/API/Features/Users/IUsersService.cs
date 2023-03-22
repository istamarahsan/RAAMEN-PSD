using System;
using System.Collections.Generic;
using PSD_Project.API.Features.Users.Authorization;
using Util.Try;

namespace PSD_Project.API.Features.Users
{
    public interface IUsersService
    {
        Try<User, Exception> GetUser(int userId);
        Try<List<User>, Exception> GetUsers();
        Try<List<User>, Exception> GetUsersWithRole(int roleId);
        Try<User, Exception> GetUserWithUsername(string username);
        Try<User, Exception> CreateUser(UserDetails userDetails);
        Try<User, Exception> UpdateUser(int userId, UserUpdateDetails form);
    }
}