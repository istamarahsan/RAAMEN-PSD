using System.Collections.Generic;
using PSD_Project.API.Features.Users;
using Util.Option;
using Util.Try;

namespace PSD_Project.App.Services.Users
{
    public interface IUserService
    {
        Try<List<User>, UserServiceError>  GetCustomers(int token);
        Try<List<User>, UserServiceError>  GetStaff(int token);
        Try<User, UserServiceError> UpdateUserDetails(int token, int userId, UserUpdateDetails updateDetails);
    }
}