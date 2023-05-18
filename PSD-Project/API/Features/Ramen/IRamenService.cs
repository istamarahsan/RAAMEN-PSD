using System;
using System.Collections.Generic;
using Util.Option;
using Util.Try;

namespace PSD_Project.API.Features.Ramen
{
    public interface IRamenService
    {
        Try<List<Ramen>, Exception> GetRamen();
        Try<Ramen, Exception> GetRamen(int id);
        Try<Ramen, Exception> CreateRamen(RamenDetails ramenDetails);
        Try<Ramen, Exception> UpdateRamen(int id, RamenDetails ramenDetails);
        Option<Exception> DeleteRamen(int id);
        Try<List<Meat>, Exception> GetMeats();
    }
}