using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PSD_Project.App.Models;
using PSD_Project.App.Pages;
using PSD_Project.Features.Users;
using Util.Try;

namespace PSD_Project.App
{
    public interface IUsersService
    {
        Task<Try<List<User>, Exception>> TryGetUsersWithRoleAsync(int roleId);
    }
}