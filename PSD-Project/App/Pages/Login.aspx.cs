using System;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using Newtonsoft.Json;

namespace PSD_Project.App.Pages
{
    public partial class Login : Page
    {
        private static readonly Uri LoginServiceUri = new Uri("http://localhost:5000/api/users/login");

        protected void Page_Load(object sender, EventArgs e)
        {
            var usernameCookie = Request.Cookies["raamen-username"];
            var passwordCookie = Request.Cookies["raamen-password"];
            if (usernameCookie != null && passwordCookie != null)
            {
                UsernameTextBox.Text = usernameCookie.Value;
                PasswordTextBox.Text = passwordCookie.Value;
            }
        }

        protected void OnClick(object sender, EventArgs e)
        {
            var credentials = new LoginCredentials(
                UsernameTextBox.Text,
                PasswordTextBox.Text);

            var json = JsonConvert.SerializeObject(credentials, Formatting.None);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var responseTask = RaamenApp.HttpClient.PostAsync(LoginServiceUri, content);
            while (responseTask.Status == TaskStatus.Running)
            {
            }

            var response = responseTask.Result;
            var loginWasSuccessful = response.StatusCode == HttpStatusCode.OK;
            LoginResultLabel.Text = loginWasSuccessful ? "Login Success" : "Something went wrong";
            if (RememberMeCheckBox.Checked)
            {
                var usernameCookie = new HttpCookie("raamen-username")
                {
                    Value = credentials.Username,
                    Expires = DateTime.Now.AddDays(1)
                };
                var passwordCookie = new HttpCookie("raamen-password")
                {
                    Value = credentials.Password,
                    Expires = DateTime.Now.AddDays(1)
                };
                Response.SetCookie(passwordCookie);
                Response.SetCookie(usernameCookie);
            }
        }

        [DataContract]
        private class LoginCredentials
        {
            [DataMember] public readonly string Password;

            [DataMember] public readonly string Username;

            public LoginCredentials(string username, string password)
            {
                Username = username;
                Password = password;
            }
        }
    }
}