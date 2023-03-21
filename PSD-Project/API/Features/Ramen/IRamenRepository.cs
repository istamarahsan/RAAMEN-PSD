using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Util.Option;
using Util.Try;

namespace PSD_Project.API.Features.Ramen
{
    public interface IRamenRepository
    {
        Task<Try<List<Ramen>, Exception>> GetRamen();
        Task<Option<Ramen>> GetRamen(int ramenId);
        Task<Try<Ramen, Exception>> AddRamen(string name, string borth, string price, int meatId);
        Task<Try<Ramen, Exception>> UpdateRamen(int ramenId, string name, string borth, string price, int meatId);
        Task<Option<Exception>> DeleteRamen(int ramenId);
    }
}