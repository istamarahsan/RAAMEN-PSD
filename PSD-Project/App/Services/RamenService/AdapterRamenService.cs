using System.Collections.Generic;
using System.Web.Http.Results;
using PSD_Project.API.Features.Ramen;
using PSD_Project.App.Common;
using Util.Option;
using Util.Try;

namespace PSD_Project.App.Services.RamenService
{
    public class AdapterRamenService : IRamenService
    {
        private readonly RamenController ramenController = new RamenController();
        
        public Try<List<Ramen>, RamenServiceError> GetRamen()
        {
            return ramenController.GetAllRamen()
                .InterpretAs<List<Ramen>>()
                .MapErr(_ => RamenServiceError.InternalServiceError);
        }

        public Try<Ramen, RamenServiceError> GetRamen(int ramenId)
        {
            return ramenController.GetRamen(ramenId)
                .InterpretAs<Ramen>()
                .MapErr(_ => RamenServiceError.InternalServiceError);
        }

        public Try<Ramen, RamenServiceError> CreateRamen(RamenDetails ramenDetails)
        {
            return ramenController.AddRamen(ramenDetails)
                .InterpretAs<Ramen>()
                .MapErr(_ => RamenServiceError.InternalServiceError);
        }

        public Try<Ramen, RamenServiceError> UpdateRamen(int ramenId, RamenDetails newDetails)
        {
            return ramenController.UpdateRamen(ramenId, newDetails)
                .InterpretAs<Ramen>()
                .MapErr(_ => RamenServiceError.InternalServiceError);
        }

        public Option<RamenServiceError> DeleteRamen(int ramenId)
        {
            return ramenController.DeleteRamen(ramenId)
                .As<OkResult>()
                .OrErr(() => RamenServiceError.InternalServiceError)
                .Err();
        }
    }
}