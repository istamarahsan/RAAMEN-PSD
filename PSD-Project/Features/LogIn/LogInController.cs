using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using PSD_Project.Features.Users;

namespace PSD_Project.Features.LogIn
{
    [RoutePrefix("api/login")]
    public class LogInController : ApiController
    {
        private readonly Uri usersServiceUri = new Uri("http://localhost:5000/api/users");

        public LogInController()
        {
        }

        public LogInController(Uri usersServiceUri)
        {
            this.usersServiceUri = usersServiceUri;
        }

        [Route]
        [HttpPost]
        public async Task<IHttpActionResult> Login([FromBody] LoginCredentials credentials)
        {
            var requestForUserWithUsername =
                await RaamenApp.HttpClient.GetAsync(new Uri(usersServiceUri, $"?username={credentials.Username}"));

            if (requestForUserWithUsername.StatusCode == HttpStatusCode.NotFound) return BadRequest();

            var responseString = await requestForUserWithUsername.Content.ReadAsStringAsync();
            var user = (User)JsonConvert.DeserializeObject(responseString, typeof(User));

            return user.Password == credentials.Password
                ? (IHttpActionResult)Ok()
                : BadRequest();
        }
    }
}