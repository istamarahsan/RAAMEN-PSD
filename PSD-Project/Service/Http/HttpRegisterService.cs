using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using PSD_Project.App.Models;
using Util.Option;
using Util.Try;

namespace PSD_Project.Service.Http
{
    public class HttpRegisterService : IRegisterService
    {
        private readonly Uri registerServiceUri;
        private readonly HttpClient httpClient;

        public HttpRegisterService(Uri registerServiceUri, HttpClient httpClient)
        {
            this.registerServiceUri = registerServiceUri;
            this.httpClient = httpClient;
        }

        public async Task<HttpStatusCode> RegisterNewUser(RegistrationFormDetails form)
        {
            var json = JsonConvert.SerializeObject(form, Formatting.None);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(registerServiceUri, content);
            return response.StatusCode;
        }
    }
}