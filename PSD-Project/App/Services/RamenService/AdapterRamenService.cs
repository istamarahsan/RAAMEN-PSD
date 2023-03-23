using System.Collections.Generic;
using System.Web.Http;
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
                .MapErr(HandleError);
        }

        public Try<Ramen, RamenServiceError> GetRamen(int ramenId)
        {
            return ramenController.GetRamen(ramenId)
                .InterpretAs<Ramen>()
                .MapErr(HandleError);
        }

        public Try<Ramen, RamenServiceError> CreateRamen(int token, RamenDetails ramenDetails)
        {
            return ramenController.WithAuthToken(token, controller =>
                controller.AddRamen(ramenDetails)
                    .InterpretAs<Ramen>()
                    .MapErr(HandleError));
        }

        public Try<Ramen, RamenServiceError> UpdateRamen(int token, int ramenId, RamenDetails newDetails)
        {
            return ramenController.WithAuthToken(token, controller =>
                controller.UpdateRamen(ramenId, newDetails)
                    .InterpretAs<Ramen>()
                    .MapErr(HandleError));
        }

        public Option<RamenServiceError> DeleteRamen(int token, int ramenId)
        {
            return ramenController.WithAuthToken(token, controller =>
            {
                switch (ramenController.DeleteRamen(ramenId))
                {
                    case OkResult _:
                        return Option.None<RamenServiceError>();
                    case var otherResponse:
                        return Option.Some(HandleError(otherResponse));
                }
            });
        }

        private RamenServiceError HandleError(IHttpActionResult response)
        {
            switch (response)
            {
                case NotFoundResult _: 
                    return RamenServiceError.RamenNotFound;
                default:
                    return RamenServiceError.InternalServiceError;
            }
        }
    }
}