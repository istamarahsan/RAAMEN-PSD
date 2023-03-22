using System;
using System.Web.Http;
using PSD_Project.API.Util;

namespace PSD_Project.API.Features.Ramen
{
    [RoutePrefix("api/ramen")]
    public class RamenController : ApiController
    {
        private readonly IRamenService ramenService = Services.GetRamenService();
        
        public RamenController()
        {
            
        }

        public RamenController(IRamenService ramenService)
        {
            this.ramenService = ramenService;
        }

        [Route]
        [HttpGet]
        public IHttpActionResult GetAllRamen()
        {
            var ramen = ramenService.GetRamen();
            return ramen.Match(Ok, HandleError);
        }

        [Route("{id}")]
        [HttpGet]
        public IHttpActionResult GetRamen(int id)
        {
            var ramen = ramenService.GetRamen(id);
            return ramen.Match(Ok, HandleError);
        }

        [Route]
        [HttpPost]
        public IHttpActionResult AddRamen([FromBody] RamenDetails form)
        {
            if (form == null) return BadRequest();

            var result = ramenService.CreateRamen(form);
            return result.Match(Ok, HandleError);
        }

        [Route("{id}")]
        [HttpPut]
        public IHttpActionResult UpdateRamen(int id, [FromBody] RamenDetails form)
        {
            if (form == null) return BadRequest();

            var result = ramenService.UpdateRamen(id, form);
            return result.Match(Ok, HandleError);
        }

        [Route("{id}")]
        [HttpDelete]
        public IHttpActionResult DeleteRamen(int id)
        {
            var error = ramenService.DeleteRamen(id);
            return error.Match(HandleError, Ok);
        }
        
        private IHttpActionResult HandleError(Exception exception)
        {
            switch (exception)
            {
                case ArgumentException _:
                    return BadRequest();
                default:
                    return InternalServerError(exception);
            }
        }
    }
}