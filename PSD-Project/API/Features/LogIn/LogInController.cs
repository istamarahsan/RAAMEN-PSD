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
        private readonly IUserSessionsService userSessionsService;
        private readonly IUsersService usersService;

        public LogInController()
        {
            userSessionsService = Services.GetUserSessionsService();
            usersService = Services.GetUsersService();
        }

        public LogInController(IUsersService usersService, IUserSessionsService userSessionsService)
        {
            this.usersService = usersService;
            this.userSessionsService = userSessionsService;
        }

        [Route]
        [HttpPost]
        public async Task<IHttpActionResult> Authenticate([FromBody] LoginCredentials credentials)
        {
            var user = await usersService.GetUserWithUsername(credentials.Username);

            return user
                .MapErr(_ => BadRequest() as IHttpActionResult)
                .Bind(u => u.Check(password => u.Password == credentials.Password, _ => BadRequest() as IHttpActionResult))
                .Bind(u => userSessionsService.CreateSessionForUser(u).OrErr(() => InternalServerError() as IHttpActionResult))
                .Map(Ok)
                .Match(ok => ok, err => err);
        }

        [Route]
        [HttpGet]
        public Task<IHttpActionResult> GetSession([FromUri] int sessionToken)
        {
            return Task.FromResult(
                userSessionsService.GetSession(sessionToken)
                    .Map(Ok)
                    .Cast<IHttpActionResult>()
                    .OrElse(NotFound()));
        }
    }
}