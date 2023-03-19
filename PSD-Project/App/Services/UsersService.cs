using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PSD_Project.App.Common;
using PSD_Project.App.Models;
using PSD_Project.Features.Users;
using Util.Try;

namespace PSD_Project.App
{
    public class UsersService : IUsersService
    {
        private static readonly Uri UsersServiceUri = new Uri("http://localhost:5000/api/users");
        
        public async Task<Try<List<User>, Exception>> TryGetUsersWithRoleAsync(int roleId)
        {
            var response = await RaamenApp.HttpClient.GetAsync(new Uri(UsersServiceUri, $"?roleId={roleId}"));
            return response.TryGetContent()
                .Bind(r => r.TryReadResponseString())
                .Bind(str => str.TryDeserializeJson<List<User>>());
        }
    }
}