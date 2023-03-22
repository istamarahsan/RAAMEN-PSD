using System;
using System.Collections.Generic;
using System.Linq;
using PSD_Project.API.Features.Authentication;
using PSD_Project.API.Features.Users;
using PSD_Project.API.Features.Users.Authorization;
using PSD_Project.App.Pages;
using Util.Collections;
using Util.Try;

namespace PSD_Project_Test
{
    public class TestAuthService : IAuthorizationService, IAuthenticationService
    {
        public readonly Dictionary<int, UserSessionDetails> UserSessionDetails = new Dictionary<int, UserSessionDetails>();
        
        private bool allowAllPermissions = true;
        private bool allowAllCredentials = true;
        private readonly IUsersService usersService;

        public TestAuthService(IUsersService usersService)
        {
            this.usersService = usersService;
        }

        public void AllowAllPermissions()
        {
            allowAllPermissions = true;
        }
        
        public void BlockAllPermissions()
        {
            allowAllPermissions = false;
        }
        
        public void AllowAllCredentials()
        {
            allowAllCredentials = true;
        }
        
        public void BlockAllCredentials()
        {
            allowAllCredentials = false;
        }

        public bool RoleHasPermission(Role role, Permission permission)
        {
            return allowAllPermissions;
        }

        public Try<UserSessionDetails, Exception> GetSession(int token)
        {
            return UserSessionDetails.Get(token).OrErr(() => new Exception("Token is invalid"));
        }

        public Try<UserSession, Exception> Authenticate(UserCredentials credentials)
        {
            return allowAllCredentials.Assert(true, () => true, () => new Exception("Authentication blocked"))
                .Bind(_ => usersService.GetUserWithUsername(credentials.Username))
                .Map(user =>
                {
                    var nextId = UserSessionDetails.Keys.DefaultIfEmpty(1).Max() + 1;
                    var session = new UserSessionDetails(
                        nextId,
                        user.Username,
                        user.Email,
                        user.Password,
                        user.Gender,
                        user.Role);
                    UserSessionDetails[nextId] = session;
                    return new UserSession(nextId, session);
                });
            
        }
    }
}