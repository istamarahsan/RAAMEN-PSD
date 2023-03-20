using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PSD_Project.App.Common;
using PSD_Project.Features.Ramen;
using Util.Try;

namespace PSD_Project.App
{
    public sealed class RamenService
    {
        private readonly Uri ramenServiceUri = new Uri("http://localhost:5000/ramen");
        
        public async Task<Try<List<Ramen>, Exception>> GetAllRamenAsync()
        {
            var response = await RaamenApp.HttpClient.GetAsync(ramenServiceUri);
            return response.TryGetContent()
                .Bind(r => r.TryReadResponseString())
                .Bind(str => str.TryDeserializeJson<List<Ramen>>());
        }
    }
}