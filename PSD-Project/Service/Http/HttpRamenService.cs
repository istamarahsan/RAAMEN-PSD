using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using PSD_Project.API.Features.Ramen;
using PSD_Project.App.Common;
using Util.Try;

namespace PSD_Project.Service.Http
{
    public sealed class HttpRamenService : IRamenService
    {
        private readonly Uri ramenServiceUri;
        private readonly HttpClient httpClient;

        public HttpRamenService(Uri ramenServiceUri, HttpClient httpClient)
        {
            this.ramenServiceUri = ramenServiceUri;
            this.httpClient = httpClient;
        }

        public async Task<Try<List<Ramen>, Exception>> GetAllRamen()
        {
            var response = await httpClient.GetAsync(ramenServiceUri);
            return response.TryGetContent()
                .Bind(r => r.TryReadResponseString())
                .Bind(str => str.TryDeserializeJson<List<Ramen>>());
        }
    }
}