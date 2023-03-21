using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using PSD_Project.API.Features.LogIn;
using PSD_Project.App.Common;
using PSD_Project.App.Pages;
using Util.Try;

namespace PSD_Project.Service.Http
{
    public class HttpAuthService : IAuthService
    {
        private readonly Uri serviceUri;
        private readonly HttpClient httpClient;

        public HttpAuthService(Uri serviceUri, HttpClient httpClient)
        {
            this.serviceUri = serviceUri;
            this.httpClient = httpClient;
        }

        public async Task<Try<UserSessionDetails, Exception>> GetSession(int token)
        {
            var response = await httpClient.GetAsync(new Uri(serviceUri, $"?sessionToken={token}"));
            return response.TryGetContent()
                .Bind(content => content.TryReadResponseString())
                .Bind(str => str.TryDeserializeJson<UserSessionDetails>());
        }

        public async Task<Try<UserSession, Exception>> Authenticate(UserCredentials credentials)
        {
            var credentialsAsJson = JsonConvert.SerializeObject(credentials, Formatting.None);
            var credentialsAsContent = new StringContent(credentialsAsJson, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(serviceUri, credentialsAsContent);
            return response.Check(
                    r => r.StatusCode == HttpStatusCode.OK,
                    r => new HttpException(r.StatusCode.ToString()))
                .MapErr(e => e as Exception)
                .Bind(r => r.TryGetContent())
                .Bind(content => content.TryReadResponseString())
                .Bind(str => str.TryDeserializeJson<UserSession>());
        }
    }
}