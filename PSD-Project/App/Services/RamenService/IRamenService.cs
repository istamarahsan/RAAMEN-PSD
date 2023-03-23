using System.Collections.Generic;
using Util.Try;
using PSD_Project.API.Features.Ramen;
using Util.Option;

namespace PSD_Project.App.Services.RamenService
{
    public interface IRamenService
    {
        Try<List<Ramen>, RamenServiceError> GetRamen();
        Try<Ramen, RamenServiceError> GetRamen(int ramenId);
        Try<Ramen, RamenServiceError> CreateRamen(int token, RamenDetails ramenDetails);
        Try<Ramen, RamenServiceError> UpdateRamen(int token, int ramenId, RamenDetails newDetails);
        Option<RamenServiceError> DeleteRamen(int token, int ramenId);
    }
}