using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace PSD_Project.Features.Ramen
{
    [RoutePrefix("api/ramen")]
    public class RamenController : ApiController
    {
        private readonly IRamenRepository ramenRepository = new RamenRepository();
        
        public RamenController()
        {
            
        }

        public RamenController(IRamenRepository ramenRepository)
        {
            this.ramenRepository = ramenRepository;
        }

        [Route]
        [HttpGet]
        public async Task<IHttpActionResult> GetAllRamen()
        {
            var ramen = await ramenRepository.GetRamenAsync();
            return ramen.Match<IHttpActionResult>(Ok, InternalServerError);
        }

        [Route("{id}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetRamen(int id)
        {
            var ramen = await ramenRepository.GetRamenAsync(id);
            return ramen.Match<IHttpActionResult>(Ok, NotFound);
        }

        [Route]
        [HttpPost]
        public async Task<IHttpActionResult> AddRamen([FromBody] NewRamenDetails form)
        {
            if (form == null) return BadRequest();
            
            IHttpActionResult HandleAddRamenError(Exception exception)
            {
                switch (exception)
                {
                    case ArgumentException _:
                        return BadRequest();
                    default:
                        return InternalServerError();
                }
            }
            
            var result = await ramenRepository.AddRamenAsync(form.Name, form.Borth, form.Price, form.MeatId);
            return result.Match(Ok, HandleAddRamenError);
        }

        [Route("{id}")]
        [HttpPut]
        public async Task<IHttpActionResult> UpdateRamen(int id, [FromBody] NewRamenDetails form)
        {
            if (form == null) return BadRequest();
            
            IHttpActionResult HandleUpdateRamenError(Exception exception)
            {
                switch (exception)
                {
                    case ArgumentException _:
                        return NotFound();
                    default:
                        return InternalServerError();
                }
            }
            
            var result = await ramenRepository.UpdateRamenAsync(id, form.Name, form.Borth, form.Price, form.MeatId);
            return result.Match(Ok, HandleUpdateRamenError);
        }

        [Route("{id}")]
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteRamen(int id)
        {
            IHttpActionResult HandleUpdateRamenError(Exception exception)
            {
                switch (exception)
                {
                    case ArgumentException _:
                        return NotFound();
                    default:
                        return InternalServerError();
                }
            }

            var error = await ramenRepository.DeleteRamenAsync(id);
            return error.Match(HandleUpdateRamenError, Ok);
        }
    }
}