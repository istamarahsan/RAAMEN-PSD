using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Http;
using PSD_Project.Features.Users;
using Util.Option;

namespace PSD_Project.Features.LogIn
{
    [RoutePrefix("api/login")]
    public class LogInController : ApiController
    {
        private Raamen _db = new Raamen();
        
        [Route]
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
    }
}