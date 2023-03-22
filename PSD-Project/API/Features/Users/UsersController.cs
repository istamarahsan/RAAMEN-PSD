using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using PSD_Project.API.Features.Authentication;
using PSD_Project.API.Features.Users.Authorization;
using PSD_Project.API.Util;
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
            return usersService.GetUsers().Match(Ok, HandleException);
        }

        [Route("{id}")]
        [HttpGet]
        public IHttpActionResult GetUser(int id)
        {
            return usersService.GetUser(id).Match(Ok, HandleException);
        }
        
        [Route]
        [HttpGet]
        public IHttpActionResult GetUsersWithRole([FromUri] int roleId)
        {
            return Request.ExtractAuthToken()
                .Bind(authenticationService.GetSession)
                .Map(user => user.Role.Id)
                .Bind(VerifyRoleId)
                .Bind(requesterRole => VerifyRoleId(roleId).Map(targetRole => (requesterRole, targetRole)))
                .Bind(request => VerifyPermissionToViewTargetRoleExists(request.targetRole).Map(permission => (request.requesterRole, permission)))
                .Map(request => authorizationService.RoleHasPermission(request.requesterRole, request.permission))
                .Bind(hasPermission =>
                    hasPermission
                        ? usersService.GetUsersWithRole(roleId)
                        : Try.Err<List<User>, Exception>(new UnauthorizedAccessException()))
                .Match(Ok, HandleException);
        }

        [Route]
        [HttpGet]
        public IHttpActionResult GetUserWithUsername([FromUri] string username)
        {
            return usersService.GetUserWithUsername(username).Match(Ok, HandleException);
        }

        [Route]
        [HttpPost]
        public IHttpActionResult CreateNewUser([FromBody] UserDetails form)
        {
            return form.ToOption()
                .OrErr<UserDetails, Exception>(() => new ArgumentException())
                .Bind(usersService.CreateUser)
                .Match(Ok, HandleException);
        }

        [Route("{id}")]
        [HttpPut]
        public IHttpActionResult UpdateUser(int id, [FromBody] UserUpdateDetails form)
        {
            return form.ToOption()
                .OrErr<UserUpdateDetails, Exception>(() => new ArgumentException())
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

        private Option<Permission> ParseViewPermissionFromTargetRole(Role role)
        {
            switch (role)
            {
                case Role.Customer:
                    return Option.Some(Permission.ReadCustomerUserdetails);
                case Role.Staff:
                    return Option.Some(Permission.ReadStaffUserdetails);
                case Role.Admin:
                default:
                    return Option.None<Permission>();
            }
        }
        
        private Try<Permission, Exception> VerifyPermissionToViewTargetRoleExists(Role role)
        {
            return ParseViewPermissionFromTargetRole(role)
                .OrErr(() => new Exception("No such permission exists"));
        }

        private Try<Role, Exception> VerifyRoleId(int roleId)
        {
            return authorizationService.RoleOfId(roleId).OrErr(() => new Exception());
        }
    }
}