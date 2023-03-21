using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using PSD_Project.API.Features.Users;
using PSD_Project.API.Service;
using Util.Try;

namespace PSD_Project.API.Features.Register
{
    [RoutePrefix("api/register")]
    public partial class RegisterController : ApiController
    {
        private readonly IUsersService usersService;

        public RegisterController()
        {
            usersService = Services.GetUsersService();
        }

        public RegisterController(IUsersService usersService)
        {
            this.usersService = usersService;
        }

        [Route]
        [HttpPost]
        public IHttpActionResult Register([FromBody] RegistrationFormDetails form)
        {
            if (form == null) return BadRequest();

            var newUserDetails = new UserDetails(username: form.Username, email: form.Email, password: form.Password, gender: form.Gender, 0);
            
            var requestForUsersWithSameUsername = usersService.GetUserWithUsername(form.Username);
            var createUser = requestForUsersWithSameUsername.Err()
                .OrErr(() => new Exception("username already exists"))
                .Bind(_ => usersService.CreateUser(newUserDetails));

            return createUser.Match<IHttpActionResult>(Ok, _ => InternalServerError());
        }
    }
}