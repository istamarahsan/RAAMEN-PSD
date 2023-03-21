using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Util.Option;
using Util.Try;

namespace PSD_Project.API.Features.Users
{
    public interface IUserRepository
    {
        Try<User, Exception> GetUser(int userId);
        Try<List<User>, Exception> GetUsers();
        Try<List<User>, Exception> GetUsersWithRole(int roleId);
        Try<List<User>, Exception> GetUsersWithUsername(string username);
        Try<User, Exception> AddNewUser(string username, string email, string password, string gender, int roleId);
        Try<User, Exception> UpdateUser(int userId, string username, string email, string gender);
    }
}