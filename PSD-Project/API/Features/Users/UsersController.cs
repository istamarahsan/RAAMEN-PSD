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
            var user = usersService.GetUser(id);
            return user.Match(Ok, HandleException);
        }
        
        [Route]
        [HttpGet]
        public IHttpActionResult GetUsersWithRole([FromUri] int roleId)
        {
            return Request.ExtractAuthToken()
                .Bind(token => authenticationService.GetSession(token))
                .Map(user => user.Role.Id)
                .Bind(requesterRoleId => usersService.GetRoleOfId(requesterRoleId))
                .Bind(requesterRole => usersService.GetRoleOfId(roleId).Map(targetRole => (requesterRole, targetRole)))
                .Bind(request => VerifyPermissionToViewTargetRoleExists(request.targetRole).Map(permission => (request.requesterRole, permission)))
                .Map(request => authorizationService.RoleHasPermission(request.requesterRole, request.permission))
                .Bind(hasPermission =>
                    hasPermission
                        ? usersService.GetUsersWithRole(roleId)
                        : Try.Err<List<User>, Exception>(new Exception("Unauthorized")))
                .Match(Ok, HandleException);
        }

        [Route]
        [HttpGet]
        public IHttpActionResult GetUserWithUsername([FromUri] string username)
        {
            var user = usersService.GetUserWithUsername(username);
            return user.Match(Ok, HandleException);
        }

        [Route]
        [HttpPost]
        public IHttpActionResult CreateNewUser([FromBody] UserDetails form)
        {
            return form.ToOption()
                .OrErr<UserDetails, IHttpActionResult>(BadRequest)
                .Bind(u => usersService.CreateUser(u).MapErr(HandleException))
                .Match(Ok, err => err);
        }

        [Route("{id}")]
        [HttpPut]
        public IHttpActionResult UpdateUser(int id, [FromBody] UserUpdateDetails form)
        {
            if (form == null) return BadRequest();
            var updateTry = usersService.UpdateUser(id, form);
            return updateTry.Match(Ok, HandleException);
        }

        private IHttpActionResult HandleException(Exception exception)
        {
            switch (exception)
            {
                case ArgumentException _:
                    return BadRequest();
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
    }
}