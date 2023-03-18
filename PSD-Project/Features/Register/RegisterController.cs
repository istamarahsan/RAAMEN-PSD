using System;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using Newtonsoft.Json;

namespace PSD_Project.Features.Register
{
    [RoutePrefix("api/register")]
    public class RegisterController : ApiController
    {
        [DataContract]
        public class RegistrationFormDetails
        {
            [DataMember]
            public string Username { get; set; }
            [DataMember]
            public string Email { get; set; }
            [DataMember]
            public string Gender { get; set; }
            [DataMember]
            public string Password { get; set; }

            public RegistrationFormDetails(string username, string email, string gender, string password)
            {
                Username = username;
                Email = email;
                Gender = gender;
                Password = password;
            }
        }

        private readonly Uri usersServiceUri = new Uri("http://localhost:5000/api/users");

        [Route]
        [HttpPost]
        public async Task<IHttpActionResult> Register([FromBody] RegistrationFormDetails form)
        {
            if (form == null) return BadRequest();

            var requestForUsersWithSameUsername = await RaamenApp.HttpClient.GetAsync(new Uri(usersServiceUri, $"?username={form.Username}"));
            if (requestForUsersWithSameUsername.StatusCode == HttpStatusCode.NotFound)
            {
                var userDetailsJson = JsonConvert.SerializeObject(form, Formatting.None);
                var userDetailsContent = new StringContent(userDetailsJson, Encoding.UTF8, "application/json");
                var requestToAddNewUser = await RaamenApp.HttpClient.PostAsync(usersServiceUri, userDetailsContent);
                return requestToAddNewUser.StatusCode == HttpStatusCode.OK
                    ? (IHttpActionResult)Ok()
                    : InternalServerError();
            }

            return Conflict();
        }
    }
}