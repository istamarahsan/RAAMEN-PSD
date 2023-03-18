using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using Newtonsoft.Json;
using PSD_Project.Features.Users;
using Util.Try;

namespace PSD_Project.Features.LogIn
{
    [RoutePrefix("api/login")]
    public class LogInController : ApiController
    {
        private readonly IUserSessions userSessions = new UserSessions();
        private readonly Uri usersServiceUri = new Uri("http://localhost:5000/api/users");

        public LogInController()
        {
        }

        public LogInController(Uri usersServiceUri, IUserSessions userSessions)
        {
            this.usersServiceUri = usersServiceUri;
            this.userSessions = userSessions;
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

            return user.Password
                .Check(password => password == credentials.Password, _ => BadRequest() as IHttpActionResult)
                .Bind(_ => userSessions.CreateSessionForUser(user).Failed(() => InternalServerError() as IHttpActionResult))
                .Map(Ok)
                .Match(ok => ok, err => err);
        }

        [Route]
        [HttpGet]
        public Task<IHttpActionResult> GetSession([FromUri] int sessionToken)
        {
            return Task.FromResult(
                userSessions.GetSession(sessionToken)
                    .Map(Ok)
                    .Cast<IHttpActionResult>()
                    .OrElse(NotFound()));
        }
    }
}