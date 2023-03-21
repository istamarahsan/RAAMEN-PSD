using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Util.Option;
using Util.Try;

namespace PSD_Project.API.Features.Ramen
{
    public interface IRamenRepository
    {
        Task<Try<List<Ramen>, Exception>> GetRamenAsync();
        Task<Option<Ramen>> GetRamenAsync(int ramenId);
        Task<Try<Ramen, Exception>> AddRamenAsync(string name, string borth, string price, int meatId);
        Task<Try<Ramen, Exception>> UpdateRamenAsync(int ramenId, string name, string borth, string price, int meatId);
        Task<Option<Exception>> DeleteRamenAsync(int ramenId);
    }
}