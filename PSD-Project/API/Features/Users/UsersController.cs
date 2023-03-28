using System;
using System.Collections.Generic;
using System.Web.Http;
using PSD_Project.API.Features.Authentication;
using PSD_Project.API.Features.Users.Authorization;
using PSD_Project.API.Util.ApiController;
using Util.Option;
using Util.Try;

namespace PSD_Project.API.Features.Users
{
    [RoutePrefix("api/users")]
    public class UsersController : ApiController
    {
        private readonly IAuthenticationService authenticationService;
        private readonly IAuthorizationService authorizationService;
        private readonly IUsersService usersService;

        public UsersController()
        {
            usersService = Services.GetUsersService();
            authenticationService = Services.GetAuthenticationService();
            authorizationService = Services.GetAuthorizationService();
        }

        public UsersController(IUsersService usersService, IAuthenticationService authenticationService,
            IAuthorizationService authorizationService)
        {
            this.usersService = usersService;
            this.authenticationService = authenticationService;
            this.authorizationService = authorizationService;
        }
        

        [Route]
        [HttpGet]
        public IHttpActionResult GetAllUsers()
        {
            return Request.ExtractAuthToken()
                .Bind(authenticationService.GetSession)
                .Map(session => session.Role.Id)
                .Map(roleId => authorizationService.RoleHasPermission(roleId, Permission.ViewAllUserdetails))
                .Bind(hasPermission =>
                    hasPermission
                        ? usersService.GetUsers()
                        : Try.Err<List<User>, Exception>(new UnauthorizedAccessException()))
                .Match(Ok, HandleException);
        }

        [Route("{id}")]
        [HttpGet]
        public IHttpActionResult GetUser(int id)
        {
            var targetPermission = usersService.GetUser(id)
                .Map(u => u.Role.Id)
                .Bind(roleId => authorizationService.PermissionToRead(roleId).OrErr<Permission, Exception>(() => new ArgumentException()));

            return Request.ExtractAuthToken()
                .Bind(authenticationService.GetSession)
                .Map(session => session.Role.Id)
                .Bind(roleId =>
                    targetPermission.Map(permission => authorizationService.RoleHasPermission(roleId, permission)))
                .Bind(hasPermission =>
                    hasPermission
                        ? usersService.GetUser(id)
                        : Try.Err<User, Exception>(new UnauthorizedAccessException()))
                .Match(Ok, HandleException);
        }
        
        [Route]
        [HttpGet]
        public IHttpActionResult GetUsersWithRole([FromUri] int roleId)
        {
            return Request.ExtractAuthToken()
                .Bind(authenticationService.GetSession)
                .Map(user => user.Role.Id)
                .Bind(requesterRoleId => authorizationService.PermissionToRead(roleId)
                    .OrErr<Permission, Exception>(() => new ArgumentException())
                    .Map(permission => authorizationService.RoleHasPermission(requesterRoleId, permission)))
                .Bind(hasPermission =>
                    hasPermission
                        ? usersService.GetUsersWithRole(roleId)
                        : Try.Err<List<User>, Exception>(new UnauthorizedAccessException()))
                .Match(Ok, HandleException);
        }

        [Route]
        [HttpPost]
        public IHttpActionResult CreateNewUser([FromBody] UserDetails form)
        {
            return Request.ExtractAuthToken()
                .Bind(authenticationService.GetSession)
                .Bind(user => authorizationService.RoleHasPermission(user.Role.Id, Permission.CreateUser)
                    .AssertTrue<UserDetails, Exception>(() => form, () => new UnauthorizedAccessException()))
                .Bind(f => f.ToOption().OrErr<UserDetails, Exception>(() => new ArgumentException()))
                .Bind(usersService.CreateUser)
                .Match(Ok, HandleException);
        }

        [Route("{id}")]
        [HttpPut]
        public IHttpActionResult UpdateUser(int id, [FromBody] UserDetails form)
        {
            return Request.ExtractAuthToken()
                .Bind(authenticationService.GetSession)
                .Bind(user => authorizationService.RoleHasPermission(user.Role.Id, Permission.UpdateUser)
                    .AssertTrue<UserDetails, Exception>(() => form, () => new UnauthorizedAccessException()))
                .Bind(details => details.ToOption().OrErr<UserDetails, Exception>(() => new ArgumentException()))
                .Bind(details => usersService.UpdateUser(id, details))
                .Match(Ok, HandleException);
        }

        private IHttpActionResult HandleException(Exception exception)
        {
            switch (exception)
            {
                case ArgumentException _:
                    return BadRequest();
                case UnauthorizedAccessException _:
                    return Unauthorized();
                default:
                    return InternalServerError(exception);
            }
        }
    }
}