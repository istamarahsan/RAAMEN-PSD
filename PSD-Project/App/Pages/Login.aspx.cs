using System;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;

namespace PSD_Project.App.Pages
{
    public partial class Login : Page
    {
        [DataContract]
        private class LoginCredentials
        {
            [DataMember]
            public readonly string Username;
            [DataMember]
            public readonly string Password;

            public LoginCredentials(string username, string password)
            {
                Username = username;
                Password = password;
            }
        }
        
        private static readonly Uri LoginServiceUri = new Uri("http://localhost:5000/api/users/login");
        
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void OnClick(object sender, EventArgs e)
        {
            var credentials = new LoginCredentials(
                UsernameTextBox.Text,
                PasswordTextBox.Text);

            var json = JsonConvert.SerializeObject(credentials, Formatting.None);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var responseTask = RaamenApp.HttpClient.PostAsync(LoginServiceUri, content);
            while (responseTask.Status == TaskStatus.Running) { }
            var response = responseTask.Result;
            var loginWasSuccessful = response.StatusCode == HttpStatusCode.OK;
            LoginResultLabel.Text = loginWasSuccessful ? "Login Success" : "Something went wrong";
        }
    }
}