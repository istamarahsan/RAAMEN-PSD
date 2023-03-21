using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Util.Option;
using Util.Try;

namespace PSD_Project.API.Features.Users
{
    public interface IUsersRepository
    {
        Task<Option<User>> GetUser(int userId);
        Task<List<User>> GetUsers();
        Task<List<User>> GetUsersWithRole(int roleId);
        Task<Option<User>> GetUserWithUsername(string username);
        Task<Try<User, Exception>> AddNewUser(string username, string email, string password, string gender, int roleId);
        Task<Try<User, Exception>> UpdateUser(int userId, string username, string email, string gender);
    }
}