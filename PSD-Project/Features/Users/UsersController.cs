using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using Util.Option;

namespace PSD_Project.Features.Users
{
    [RoutePrefix("api/users")]
    public class UsersController : ApiController
    {
        private readonly IUsersRepository usersRepository;

        public UsersController()
        {
            usersRepository = new UsersRepository();
        }

        public UsersController(IUsersRepository usersRepository)
        {
            this.usersRepository = usersRepository;
        }

        [Route]
        [HttpGet]
        public async Task<List<User>> GetAllUsers()
        {
            return await usersRepository.GetUsersAsync();
        }
        
        [Route("{id}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetUser(int id)
        {
            var user = await usersRepository.GetUserAsync(id);
            return user
                .Map(Ok)
                .Cast<IHttpActionResult>()
                .OrElse(NotFound());
        }

        [Route]
        [HttpGet]
        public async Task<List<User>> GetUsersWithRole([FromUri] int roleId)
        {
            return await usersRepository.GetUsersWithRoleAsync(roleId);
        }

        [Route]
        [HttpGet]
        public async Task<IHttpActionResult> GetUserWithUsername([FromUri] string username)
        {
            var user = await usersRepository.GetUserWithUsernameAsync(username);
            return user.Map(Ok)
                .Cast<IHttpActionResult>()
                .OrElse(NotFound());
        }
        
        [Route]
        [HttpPost]
        public async Task<IHttpActionResult> CreateNewUser([FromBody] NewUserDetails form)
        {
            IHttpActionResult HandleAddException(Exception e)
            {
                switch (e)
                {
                    case ArgumentException _:
                        return BadRequest();
                    default:
                        return InternalServerError();
                }
            }
            
            if (form == null) return BadRequest();
            var userTry = await usersRepository.AddNewUserAsync(username: form.Username, email: form.Email,
                password: form.Password, gender: form.Gender, roleId: form.RoleId);
            return userTry.Match(Ok, HandleAddException);
        }
    }
}