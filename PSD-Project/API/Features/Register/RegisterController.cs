using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using PSD_Project.API.Features.Users;

namespace PSD_Project.API.Features.Register
{
    [RoutePrefix("api/register")]
    public partial class RegisterController : ApiController
    {
        private readonly Uri usersServiceUri = new Uri("http://localhost:5000/api/users");

        public RegisterController()
        {
        }

        public RegisterController(Uri usersServiceUri)
        {
            this.usersServiceUri = usersServiceUri;
        }

        [Route]
        [HttpPost]
        public async Task<IHttpActionResult> Register([FromBody] RegistrationFormDetails form)
        {
            if (form == null) return BadRequest();

            var requestForUsersWithSameUsername =
                await RaamenApp.HttpClient.GetAsync(new Uri(usersServiceUri, $"?username={form.Username}"));
            if (requestForUsersWithSameUsername.StatusCode == HttpStatusCode.NotFound)
            {
                var userDetailsJson = JsonConvert.SerializeObject(
                    new NewUserDetails(
                        username: form.Username,
                        email: form.Email,
                        password: form.Password,
                        gender: form.Gender,
                        0), 
                    Formatting.None);
                var userDetailsContent = new StringContent(userDetailsJson, Encoding.UTF8, "application/json");
                var requestToAddNewUser = await RaamenApp.HttpClient.PostAsync(usersServiceUri, userDetailsContent);
                return requestToAddNewUser.StatusCode == HttpStatusCode.OK
                    ? (IHttpActionResult)Ok()
                    : InternalServerError();
            }

            return Conflict();
        }
    }
}