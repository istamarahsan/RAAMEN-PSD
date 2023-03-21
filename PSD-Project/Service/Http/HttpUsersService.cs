using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PSD_Project.API.Features.Users;
using PSD_Project.App.Common;
using PSD_Project.App.Models;
using Util.Option;
using Util.Try;

namespace PSD_Project.Service.Http
{
    public class HttpUsersService : IUsersService
    {
        private readonly Uri usersServiceUri;
        private readonly HttpClient httpClient;

        public HttpUsersService(Uri usersServiceUri, HttpClient httpClient)
        {
            this.usersServiceUri = usersServiceUri;
            this.httpClient = httpClient;
        }


        public async Task<Try<List<User>, Exception>> GetUsersWithRole(int roleId)
        {
            var response = await httpClient.GetAsync(new Uri(usersServiceUri, $"?roleId={roleId}"));
            return response.TryGetContent()
                .Bind(r => r.TryReadResponseString())
                .Bind(str => str.TryDeserializeJson<List<User>>());
        }

        public async Task<Try<User, Exception>> GetUserWithUsername(string username)
        {
            var response = await httpClient.GetAsync(new Uri(usersServiceUri, $"?username={username}"));
            
            if (response.StatusCode == HttpStatusCode.NotFound) return Try.Err<User, Exception>(new Exception());

            var responseString = await response.Content.ReadAsStringAsync();
            var deserialized = (User)JsonConvert.DeserializeObject(responseString, typeof(User));
            return deserialized.ToOption().OrErr(() => new Exception());
        }

        public async Task<Try<User, Exception>> CreateUser(NewUserDetails userDetails)
        {
            var userDetailsJson = JsonConvert.SerializeObject(userDetails, Formatting.None);
            var userDetailsContent = new StringContent(userDetailsJson, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(usersServiceUri, userDetailsContent);
            return response.TryGetContent()
                .Bind(r => r.TryReadResponseString())
                .Bind(str => str.TryDeserializeJson<User>());
        }

        public async Task<HttpStatusCode> UpdateUser(int userId, UserUpdateDetails form)
        {
            var jsonString = JsonConvert.SerializeObject(form, Formatting.None);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await httpClient.PutAsync(new Uri(usersServiceUri + $"/{userId}"), content);
            return response.StatusCode;
        }

        public async Task<Try<User, Exception>> GetUser(int userId)
        {
            var response = await httpClient.GetAsync(new Uri(usersServiceUri + $"/{userId}"));
            return response.TryGetContent()
                .Bind(r => r.TryReadResponseString())
                .Bind(str => str.TryDeserializeJson<User>());
        }
    }
}