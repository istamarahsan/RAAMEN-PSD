using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using PSD_Project.API.Features.Users;
using PSD_Project.App.Models;
using PSD_Project.App.Pages;
using Util.Try;

namespace PSD_Project.Service
{
    public interface IUsersService
    {
        Task<Try<List<User>, Exception>> GetUsersWithRole(int roleId);
        Task<Try<User, Exception>> GetUserWithUsername(string username);
        Task<Try<User, Exception>> CreateUser(NewUserDetails userDetails);
        Task<HttpStatusCode> UpdateUser(int userId, UserUpdateDetails form);
        Task<Try<User, Exception>> GetUser(int userId);
    }
}