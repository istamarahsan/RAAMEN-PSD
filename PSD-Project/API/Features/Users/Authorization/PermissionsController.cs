using System;
using System.IO.Ports;
using System.Linq;
using System.Web.Http;
using PSD_Project.API.Features.Authentication;
using PSD_Project.API.Service;

namespace PSD_Project.API.Features.Users.Authorization
{
    [RoutePrefix("api/permissions")]
    public class PermissionsController : ApiController
    {
        private readonly IAuthenticationService authenticationService;
        private readonly IAuthorizationService authorizationService;
        private readonly IUsersService usersService;

        public PermissionsController()
        {
            authenticationService = Services.GetAuthenticationService();
            authorizationService = Services.GetAuthorizationService();
            usersService = Services.GetUsersService();
        }
        
        public PermissionsController(IAuthenticationService authenticationService, IAuthorizationService authorizationService, IUsersService usersService)
        {
            this.authenticationService = authenticationService;
            this.authorizationService = authorizationService;
            this.usersService = usersService;
        }
        
        [Route]
        [HttpGet]
        public IHttpActionResult GetPermissions([FromUri] int token)
        {
            return authenticationService.GetSession(token)
                .Bind(userSession => usersService.GetUser(userSession.Id))
                .Map(user => user.Role.Id)
                .Bind(roleId => usersService.GetRoleOfId(roleId))
                .Map(role => authorizationService.GetPermissions(role))
                .Map(permissions => permissions.Select(p => p.ToString()).ToList())
                .Match(Ok, HandleException);
        }

        private IHttpActionResult HandleException(Exception exception)
        {
            return InternalServerError(exception);
        }
    }
}