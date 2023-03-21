using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using PSD_Project.API.Features.Users;
using PSD_Project.Service;
using Util.Try;

namespace PSD_Project.API.Features.LogIn
{
    [RoutePrefix("api/login")]
    public class LogInController : ApiController
    {
        private readonly IUserSessions userSessions = new UserSessions();
        private readonly IUsersService usersService = Services.GetUsersService();

        public LogInController()
        {
        }

        public LogInController(IUsersService usersService, IUserSessions userSessions)
        {
            this.usersService = usersService;
            this.userSessions = userSessions;
        }

        [Route]
        [HttpPost]
        public async Task<IHttpActionResult> Authenticate([FromBody] LoginCredentials credentials)
        {
            var user = await usersService.GetUserWithUsername(credentials.Username);

            return user
                .MapErr(_ => BadRequest() as IHttpActionResult)
                .Bind(u => u.Check(password => u.Password == credentials.Password, _ => BadRequest() as IHttpActionResult))
                .Bind(u => userSessions.CreateSessionForUser(u).OrErr(() => InternalServerError() as IHttpActionResult))
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