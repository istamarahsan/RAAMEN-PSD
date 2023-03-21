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
using Util.Try;

namespace PSD_Project.Services
{
    public class UsersService : IUsersService
    {
        private static readonly Uri UsersServiceUri = new Uri("http://localhost:5000/api/users");
        
        public async Task<Try<List<User>, Exception>> GetUsersWithRoleAsync(int roleId)
        {
            var response = await RaamenApp.HttpClient.GetAsync(new Uri(UsersServiceUri, $"?roleId={roleId}"));
            return response.TryGetContent()
                .Bind(r => r.TryReadResponseString())
                .Bind(str => str.TryDeserializeJson<List<User>>());
        }

        public async Task<HttpStatusCode> UpdateUserAsync(int userId, UserUpdateDetails form)
        {
            var jsonString = JsonConvert.SerializeObject(form, Formatting.None);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await RaamenApp.HttpClient.PutAsync(new Uri(UsersServiceUri + $"/{userId}"), content);
            return response.StatusCode;
        }

        public async Task<Try<User, Exception>> GetUserAsync(int userId)
        {
            var response = await RaamenApp.HttpClient.GetAsync(new Uri(UsersServiceUri + $"/{userId}"));
            return response.TryGetContent()
                .Bind(r => r.TryReadResponseString())
                .Bind(str => str.TryDeserializeJson<User>());
        }
    }
}