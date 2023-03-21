using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using PSD_Project.API.Service;
using Util.Option;
using Util.Try;

namespace PSD_Project.API.Features.Users
{
    [RoutePrefix("api/users")]
    public class UsersController : ApiController
    {
        private readonly IUsersService usersService;

        public UsersController()
        {
            usersService = Services.GetUsersService();
        }

        public UsersController(IUsersService usersService)
        {
            this.usersService = usersService;
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
            var users = usersService.GetUsersWithRole(roleId);
            return users.Match(Ok, HandleException);
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
    }
}