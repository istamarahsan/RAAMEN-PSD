using System;
using System.Web.Http;
using PSD_Project.API.Features.Authentication;
using PSD_Project.API.Features.Users.Authorization;
using PSD_Project.API.Util;
using PSD_Project.API.Util.ApiController;
using Util.Option;
using Util.Try;

namespace PSD_Project.API.Features.Ramen
{
    [RoutePrefix("api/ramen")]
    public class RamenController : ApiController
    {
        private readonly IAuthenticationService authenticationService;
        private readonly IAuthorizationService authorizationService;
        private readonly IRamenService ramenService;
        
        public RamenController()
        {
            authenticationService = Services.GetAuthenticationService();
            authorizationService = Services.GetAuthorizationService();
            ramenService = Services.GetRamenService();
        }

        public RamenController(IAuthenticationService authenticationService, IAuthorizationService authorizationService, IRamenService ramenService)
        {
            this.authenticationService = authenticationService;
            this.authorizationService = authorizationService;
            this.ramenService = ramenService;
        }

        [Route]
        [HttpGet]
        public IHttpActionResult GetAllRamen()
        {
            return ramenService.GetRamen().Match(Ok, HandleError);
        }

        [Route("{id}")]
        [HttpGet]
        public IHttpActionResult GetRamen(int id)
        {
            return ramenService.GetRamen(id).Match(Ok, HandleError);
        }

        [Route]
        [HttpPost]
        public IHttpActionResult CreateRamen([FromBody] RamenDetails form)
        {
            return Request.ExtractAuthToken()
                .Bind(authenticationService.GetSession)
                .Map(user => user.Role.Id)
                .Map(roleId => authorizationService.RoleHasPermission(roleId, Permission.AddRamen))
                .Bind(hasPermission => hasPermission
                    ? ValidateForm(form)
                    : Try.Err<RamenDetails, Exception>(new UnauthorizedAccessException()))
                .Bind(ramenService.CreateRamen)
                .Match(Ok, HandleError);
        }

        [Route("{id}")]
        [HttpPut]
        public IHttpActionResult UpdateRamen(int id, [FromBody] RamenDetails form)
        {
            return Request.ExtractAuthToken()
                .Bind(authenticationService.GetSession)
                .Map(user => user.Role.Id)
                .Map(roleId => authorizationService.RoleHasPermission(roleId, Permission.UpdateRamen))
                .Bind(hasPermission => hasPermission
                    ? ValidateForm(form)
                    : Try.Err<RamenDetails, Exception>(new UnauthorizedAccessException()))
                .Bind(details => ramenService.UpdateRamen(id, details))
                .Match(Ok, HandleError);
        }

        [Route("{id}")]
        [HttpDelete]
        public IHttpActionResult DeleteRamen(int id)
        {
            return Request.ExtractAuthToken()
                .Bind(authenticationService.GetSession)
                .Map(user => user.Role.Id)
                .Map(roleId => authorizationService.RoleHasPermission(roleId, Permission.DeleteRamen))
                .Map(hasPermission => hasPermission
                    ? ramenService.DeleteRamen(id)
                    : Option.Some<Exception>(new UnauthorizedAccessException()))
                .Recover(Option.Some)
                .Match(HandleError, Ok);
        }
        
        private IHttpActionResult HandleError(Exception exception)
        {
            switch (exception)
            {
                case ArgumentException _:
                    return BadRequest();
                case UnauthorizedAccessException _ :
                    return Unauthorized();
                default:
                    return InternalServerError(exception);
            }
        }

        private Try<RamenDetails, Exception> ValidateForm(RamenDetails form)
        {
            return form.ToOption()
                .OrErr<RamenDetails, Exception>(() => new ArgumentException());
        }
    }
}