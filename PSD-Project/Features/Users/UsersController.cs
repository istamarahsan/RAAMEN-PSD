using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Util.Option;

namespace PSD_Project.Features.Users
{
    [RoutePrefix("api/users")]
    public class UsersController : ApiController
    {
        public class UserDetails
        {
            public string Username { get; set; }
            public string Email { get; set; }
            public string Gender { get; set; }
            public string Password { get; set; }

            public UserDetails(string username, string email, string gender, string password)
            {
                Username = username;
                Email = email;
                Gender = gender;
                Password = password;
            }
        }
        
        private readonly Raamen _db = new Raamen();

        [Route]
        [HttpGet]
        public async Task<List<User>> GetAllUsers()
        {
            var users = await _db.Users.ToListAsync();
            return users.Select(ConvertModel).ToList();
        }
        
        [Route("{id}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetUser(int id)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
            return user
                .ToOption()
                .Map(ConvertModel)
                .Map(Ok)
                .Cast<IHttpActionResult>()
                .OrElse(NotFound());
        }

        [Route]
        [HttpGet]
        public async Task<List<User>> GetUsersWithRole([FromUri] int roleId)
        {
            var users = await _db.Roles
                .Where(role => role.id == roleId)
                .Join(_db.Users,
                    role => role.id,
                    user => user.Roleid,
                    (role, user) => user)
                .ToListAsync();
            return users
                .Select(ConvertModel)
                .ToList();
        }

        [Route]
        [HttpGet]
        public async Task<IHttpActionResult> GetUsersWithUsername([FromUri] string username)
        {
            var users = await _db.Users.Where(user => user.Username == username).ToListAsync();
            return users.Any()
                ? (IHttpActionResult)Ok(users)
                : NotFound();
        }
        
        [Route]
        [HttpPost]
        public async Task<IHttpActionResult> CreateNewUser([FromBody] UserDetails form)
        {
            if (form == null) return BadRequest();

            _db.Users.Add(new PSD_Project.User
            {
                Id = _db.Users.Select(users => users.Id).DefaultIfEmpty(0).Max() + 1,
                Username = form.Username,
                Email = form.Email,
                Gender = form.Gender,
                Password = form.Password,
                Roleid = 0
            });

            try
            {
                await _db.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                return InternalServerError();
            }
        }

        [Route("login")]
        [HttpPost]
        public async Task<IHttpActionResult> Login([FromBody] LoginCredentials credentials)
        {
            return (await _db.Users
                .FirstOrDefaultAsync(user =>
                    user.Username == credentials.Username
                    && user.Password == credentials.Password))
                .ToOption()
                .Map(_ => Ok())
                .Cast<IHttpActionResult>()
                .OrElse(BadRequest());
        }

        private User ConvertModel(PSD_Project.User user) => new User(user.Id, user.Username, user.Email, user.Gender);
    }
}