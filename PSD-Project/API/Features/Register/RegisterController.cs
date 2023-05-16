using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using PSD_Project.API.Features.Users;
using PSD_Project.API.Util;
using Util.Try;

namespace PSD_Project.API.Features.Register
{
    [RoutePrefix("api/register")]
    public class RegisterController : ApiController
    {
        private readonly IRegisterService registerService;

        public RegisterController()
        {
            registerService = Services.GetRegisterService();
        }

        public RegisterController(IRegisterService registerService)
        {
            this.registerService = registerService;
        }

        [Route]
        [HttpPost]
        public IHttpActionResult Register([FromBody] RegistrationFormDetails form)
        {
            if (form == null) return BadRequest();
            return registerService.Register(form)
                .Match<IHttpActionResult>(Ok, _ => InternalServerError());
        }
    }
}