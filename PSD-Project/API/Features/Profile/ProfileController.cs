using System;
using System.Web.Http;
using PSD_Project.API.Features.Authentication;
using PSD_Project.API.Features.Users;
using PSD_Project.API.Features.Users.Authorization;
using PSD_Project.API.Util.ApiController;
using PSD_Project.App.Common;
using Util.Option;
using Util.Try;

namespace PSD_Project.API.Features.Profile
{
    [RoutePrefix("api/profile")]
    public class ProfileController : ApiController
    {
        private readonly IAuthenticationService authenticationService;
        private readonly IAuthorizationService authorizationService;
        private readonly IUsersService usersService;

        public ProfileController()
        {
            usersService = Services.GetUsersService();
            authenticationService = Services.GetAuthenticationService();
            authorizationService = Services.GetAuthorizationService();
        }

        public ProfileController(IUsersService usersService, IAuthenticationService authenticationService,
            IAuthorizationService authorizationService)
        {
            this.usersService = usersService;
            this.authenticationService = authenticationService;
            this.authorizationService = authorizationService;
        }

        [Route]
        [HttpPut]
        public IHttpActionResult UpdateProfile([FromBody] ProfileDetails form)
        {
            return form.ToOption()
                .OrErr<ProfileDetails, Exception>(() => new ArgumentException())
                .Bind(_ => Request.ExtractAuthToken())
                .Bind(authenticationService.GetSession)
                .Bind(session => authorizationService.RoleHasPermission(session.Role.Id, Permission.UpdateProfile)
                    .AssertTrue<(UserSessionDetails, ProfileDetails), Exception>(() => (session, form),
                        () => new UnauthorizedAccessException()))
                .Bind(request => usersService.UpdateProfile(request.Item1.Id, request.Item2))
                .Match(Ok, HandleError);
        }

        private IHttpActionResult HandleError(Exception e)
        {
            return InternalServerError(e);
        }
    }
}