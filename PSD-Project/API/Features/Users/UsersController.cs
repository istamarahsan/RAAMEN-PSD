using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace PSD_Project.API.Features.Users
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
            return await usersRepository.GetUsers();
        }
        
        [Route("{id}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetUser(int id)
        {
            var user = await usersRepository.GetUser(id);
            return user
                .Map(Ok)
                .Cast<IHttpActionResult>()
                .OrElse(NotFound());
        }

        [Route]
        [HttpGet]
        public async Task<List<User>> GetUsersWithRole([FromUri] int roleId)
        {
            return await usersRepository.GetUsersWithRole(roleId);
        }

        [Route]
        [HttpGet]
        public async Task<IHttpActionResult> GetUserWithUsername([FromUri] string username)
        {
            var user = await usersRepository.GetUserWithUsername(username);
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
            var userTry = await usersRepository.AddNewUser(username: form.Username, email: form.Email,
                password: form.Password, gender: form.Gender, roleId: form.RoleId);
            return userTry.Match(Ok, HandleAddException);
        }

        [Route("{id}")]
        [HttpPut]
        public async Task<IHttpActionResult> UpdateUser(int id, [FromBody] UserUpdateDetails form)
        {
            IHttpActionResult HandleUpdateException(Exception e)
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
            var updateTry = await usersRepository.UpdateUser(id, form.Username, form.Email, form.Gender);
            return updateTry.Match(Ok, HandleUpdateException);
        }
    }
}