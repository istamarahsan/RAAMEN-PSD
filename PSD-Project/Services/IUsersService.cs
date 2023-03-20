using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using PSD_Project.App.Models;
using PSD_Project.App.Pages;
using PSD_Project.Features.Users;
using Util.Try;

namespace PSD_Project.Services
{
    public interface IUsersService
    {
        Task<Try<List<User>, Exception>> GetUsersWithRoleAsync(int roleId);
        Task<HttpStatusCode> UpdateUserAsync(int userId, UserUpdateDetails form);
        Task<Try<User, Exception>> GetUserAsync(int userId);
    }
}