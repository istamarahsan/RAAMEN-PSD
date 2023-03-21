using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using PSD_Project.API.Features.Users;
using PSD_Project.API.Service;
using Util.Try;

namespace PSD_Project.API.Features.LogIn
{
    [RoutePrefix("api/auth")]
    public class UserAuthenticationController : ApiController
    {
        private readonly IUserSessionsService userSessionsService;
        private readonly IUsersService usersService;

        public UserAuthenticationController()
        {
            userSessionsService = Services.GetUserSessionsService();
            usersService = Services.GetUsersService();
        }

        public UserAuthenticationController(IUsersService usersService, IUserSessionsService userSessionsService)
        {
            this.usersService = usersService;
            this.userSessionsService = userSessionsService;
        }

        [Route]
        [HttpPost]
        public IHttpActionResult Authenticate([FromBody] LoginCredentials credentials)
        {
            var user = usersService.GetUserWithUsername(credentials.Username);

            return user
                .MapErr(_ => BadRequest() as IHttpActionResult)
                .Bind(u => u.Check(password => u.Password == credentials.Password, _ => BadRequest() as IHttpActionResult))
                .Bind(u => userSessionsService.CreateSessionForUser(u).OrErr(() => InternalServerError() as IHttpActionResult))
                .Map(Ok)
                .Match(ok => ok, err => err);
        }

        [Route]
        [HttpGet]
        public IHttpActionResult GetSession([FromUri] int token)
        {
            return userSessionsService.GetSession(token)
                    .Map(Ok)
                    .Cast<IHttpActionResult>()
                    .OrElse(NotFound());
        }
    }
}