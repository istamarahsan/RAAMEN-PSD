using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PSD_Project.API.Features.Ramen;
using Util.Try;

namespace PSD_Project.Service
{
    public interface IRamenService
    {
        Task<Try<List<Ramen>, Exception>> GetAllRamen();
    }
}