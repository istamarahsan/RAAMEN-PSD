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

namespace PSD_Project.Services
{
    public class AuthService : IAuthService
    {
        private static readonly Uri ServiceUri = new Uri("http://localhost:5000/api/login");

        public async Task<Try<UserSessionDetails, Exception>> GetSession(int token)
        {
            var response = await RaamenApp.HttpClient.GetAsync(new Uri(ServiceUri, $"?sessionToken={token}"));
            return response.TryGetContent()
                .Bind(content => content.TryReadResponseString())
                .Bind(str => str.TryDeserializeJson<UserSessionDetails>());
        }

        public async Task<Try<UserSession, Exception>> Authenticate(UserCredentials credentials)
        {
            var credentialsAsJson = JsonConvert.SerializeObject(credentials, Formatting.None);
            var credentialsAsContent = new StringContent(credentialsAsJson, Encoding.UTF8, "application/json");
            var response = await RaamenApp.HttpClient.PostAsync(ServiceUri, credentialsAsContent);
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