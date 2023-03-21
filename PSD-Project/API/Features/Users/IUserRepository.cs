using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Util.Option;
using Util.Try;

namespace PSD_Project.API.Features.Users
{
    public interface IUsersRepository
    {
        Task<Option<User>> GetUserAsync(int userId);
        Task<List<User>> GetUsersAsync();
        Task<List<User>> GetUsersWithRoleAsync(int roleId);
        Task<Option<User>> GetUserWithUsernameAsync(string username);
        Task<Try<User, Exception>> AddNewUserAsync(string username, string email, string password, string gender, int roleId);
        Task<Try<User, Exception>> UpdateUserAsync(int userId, string username, string email, string gender);
    }
}