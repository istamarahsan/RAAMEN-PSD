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
        public class RegistrationFormDetails
        {
            public string Username { get; set; }
            public string Email { get; set; }
            public string Gender { get; set; }
            public string Password { get; set; }

            public RegistrationFormDetails(string username, string email, string gender, string password)
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
        [HttpPost]
        public IHttpActionResult CreateNewUser([FromBody] RegistrationFormDetails form)
        {
            if (form == null) return BadRequest();

            if (_db.Users.Select(user => user.Email).Contains(form.Email)) return StatusCode(HttpStatusCode.Conflict);

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

        private User ConvertModel(PSD_Project.User user) => new User(user.Id, user.Username, user.Email, user.Gender);
    }
}