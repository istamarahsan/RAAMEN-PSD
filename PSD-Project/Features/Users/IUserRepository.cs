using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PSD_Project.Features.Users;
using Util.Option;

namespace PSD_Project.Features
{
    public interface IUsersRepository
    {
        Task<Option<User>> GetUserAsync(int userId);
        Task<List<User>> GetUsersAsync();
        Task<List<User>> GetUsersWithRoleAsync(int roleId);
        Task<Option<User>> GetUserWithUsernameAsync(string username);
        Task AddNewUserAsync(string username, string email, string password, string gender, int roleId);
    }
}