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

namespace PSD_Project.Services
{
    public class RegisterService : IRegisterService
    {
        private static readonly Uri RegisterServiceUri = new Uri("http://localhost:5000/api/register");
        
        public async Task<HttpStatusCode> RegisterNewUserAsync(RegistrationFormDetails form)
        {
            var json = JsonConvert.SerializeObject(form, Formatting.None);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await RaamenApp.HttpClient.PostAsync(RegisterServiceUri, content);
            return response.StatusCode;
        }
    }
}