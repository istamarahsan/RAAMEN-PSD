using System.Collections.Generic;
using PSD_Project.API.Features.Users;
using Util.Option;
using Util.Try;

namespace PSD_Project.App.Services.Users
{
    public interface IUserService
    {
        Try<List<User>, UserServiceError>  GetCustomers();
        Try<List<User>, UserServiceError>  GetStaff();
        Try<User, UserServiceError> UpdateUserDetails(int userId, UserUpdateDetails updateDetails);
    }
}