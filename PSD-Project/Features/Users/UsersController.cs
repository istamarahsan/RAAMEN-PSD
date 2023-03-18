using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        public IEnumerable<User> GetAllUsers()
        {
            return _db.Users
                .Select(ConvertModel)
                .ToList();
        }
        
        [Route("{id}")]
        [HttpGet]
        public IHttpActionResult GetUser(int id)
        {
            return _db.Users
                .FirstOrDefault(u => u.Id == id)
                .ToOption()
                .Map(ConvertModel)
                .Map(Ok)
                .Cast<IHttpActionResult>()
                .OrElse(NotFound());
        }

        [Route]
        [HttpGet]
        public IEnumerable<User> GetUsersWithRole([FromUri] int roleId)
        {
            return _db.Roles
                .Where(role => role.id == roleId)
                .Join(_db.Users,
                    role => role.id,
                    user => user.Roleid,
                    (role, user) => user)
                .AsEnumerable()
                .Select(ConvertModel)
                .ToList();
        }

        [Route]
        [HttpGet]
        public IHttpActionResult GetUsersWithUsername([FromUri] string username)
        {
            var users = _db.Users.Where(user => user.Username == username).ToList();
            return users.Any()
                ? (IHttpActionResult)Ok(users)
                : NotFound();
        }
        
        [Route]
        [HttpPost]
        public IHttpActionResult CreateNewUser([FromBody] UserDetails form)
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
                _db.SaveChanges();
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
        public IHttpActionResult Login([FromBody] LoginCredentials credentials)
        {
            var credentialsMatch = _db.Users
                .FirstOrDefault(user => user.Username == credentials.Username
                                        && user.Password == credentials.Password) != null;

            return credentialsMatch ? (IHttpActionResult)Ok() : BadRequest();
        }

        private User ConvertModel(PSD_Project.User user) => new User(user.Id, user.Username, user.Email, user.Gender);
    }
}