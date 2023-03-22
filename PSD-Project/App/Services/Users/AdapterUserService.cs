using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Results;
using PSD_Project.API.Features.Users;
using PSD_Project.App.Common;
using Util.Try;

namespace PSD_Project.App.Services.Users
{
    public class AdapterUserService : IUserService
    {
        private readonly UsersController usersController = new UsersController();
        
        public Try<List<User>, UserServiceError> GetCustomers()
        {
            return usersController.GetUsersWithRole(0)
                .InterpretAs<List<User>>()
                .MapErr(HandleError);
        }

        public Try<List<User>, UserServiceError> GetStaff()
        {
            return usersController.GetUsersWithRole(1)
                .InterpretAs<List<User>>()
                .MapErr(HandleError);
        }

        public Try<User, UserServiceError> UpdateUserDetails(int userId, UserUpdateDetails updateDetails)
        {
            return usersController.UpdateUser(userId, updateDetails)
                .InterpretAs<User>()
                .MapErr(HandleError);
        }

        private UserServiceError HandleError(IHttpActionResult response)
        {
            switch (response)
            {
                case BadRequestResult _:
                    return UserServiceError.InvalidDetails;
                case NotFoundResult _:
                    return UserServiceError.UserNotFound;
                case UnauthorizedResult _:
                    return UserServiceError.PermissionDenied;
                default:
                    return UserServiceError.InternalServiceError;
            }
        }
    }
}