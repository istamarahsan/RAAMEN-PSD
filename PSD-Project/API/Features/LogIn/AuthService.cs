using System;
using PSD_Project.API.Features.Users;
using PSD_Project.App.Pages;
using Util.Try;

namespace PSD_Project.API.Features.LogIn
{
    public class AuthService : IAuthService
    {
        private readonly IUserSessionsService userSessionsService;
        private readonly IUsersService usersService;

        public AuthService(IUserSessionsService userSessionsService, IUsersService usersService)
        {
            this.userSessionsService = userSessionsService;
            this.usersService = usersService;
        }

        public Try<UserSessionDetails, Exception> GetSession(int token)
        {
            return userSessionsService.GetSession(token).OrErr(() => new Exception());
        }

        public Try<UserSession, Exception> Authenticate(UserCredentials credentials)
        {
            return usersService.GetUserWithUsername(credentials.Username)
                .Bind(user => ValidatePassword(user, credentials.Password))
                .Bind(CreateUserSession);
        }

        private Try<UserSession, Exception> CreateUserSession(User user)
        {
            return userSessionsService.CreateSessionForUser(user)
                .OrErr(() => new Exception("Could not create session"));
        }

        private Try<User, Exception> ValidatePassword(User user, string truePassword)
        {
            return user.Assert(
                u => truePassword == u.Password,
                _ => new Exception("Invalid password"));
        }
    }
}