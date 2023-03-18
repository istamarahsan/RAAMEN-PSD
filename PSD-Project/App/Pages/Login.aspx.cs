using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using Newtonsoft.Json;
using PSD_Project.App.Common;
using PSD_Project.Features.LogIn;
using Util.Option;
using Util.Try;

namespace PSD_Project.App.Pages
{
    public partial class Login : Page
    {
        private static readonly Uri LoginServiceUri = new Uri("http://localhost:5000/api/login");

        protected void Page_Load(object sender, EventArgs e)
        {

            var sessionTokenCookie = Request.Cookies["raamen-session"].ToOption();
            sessionTokenCookie.Map(cookie => cookie.Value)
                .Bind(val => Try.OfFallible<string, int>(int.Parse)(val).Ok())
                .Match(
                    some: token =>
                    {
                        if (ValidateSession(token))
                        {
                            Response.Redirect("Home.aspx");
                        }
                    },
                    none: TryFillRememberedCredentials);
        }

        protected void OnClick(object sender, EventArgs e)
        {
            var credentials = new UserCredentials(
                UsernameTextBox.Text,
                PasswordTextBox.Text);

            var credentialsAsJson = JsonConvert.SerializeObject(credentials, Formatting.None);
            var credentialsAsContent = new StringContent(credentialsAsJson, Encoding.UTF8, "application/json");
            var loginResponseTask = RaamenApp.HttpClient.PostAsync(LoginServiceUri, credentialsAsContent);
            loginResponseTask.Wait();
            loginResponseTask.Check(task => task.Status == TaskStatus.RanToCompletion, _ => "error sending http request")
                .Map(task => task.Result)
                .Bind(response => response.Check(
                    r => r.StatusCode == HttpStatusCode.OK, 
                    r => $"HTTP Error: {r.StatusCode}"))
                .Map(response => response.Content)
                .Bind(content => content.TryReadResponseString()
                    .OrErr(() => "error converting http response to string"))
                .Bind(str => str.TryDeserializeJson<UserSession>()
                    .OrErr(() => "error deserializing response"))
                .Map(s => s.SessionToken)
                .Match(
                    ok: token =>
                    {
                        LoginResultLabel.Text = "Login successful!";
                        
                        var tokenCookie = new HttpCookie("raamen-session")
                        {
                            Value = token.ToString(),
                            Expires = DateTime.Now.AddDays(1)
                        };
                        Response.SetCookie(tokenCookie);

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
                        
                        Response.Redirect("Home.aspx");
                    },
                    err: errorMessage =>
                    {
                        LoginResultLabel.Text = errorMessage;
                    });
            
        }

        private bool ValidateSession(int token)
        {
            var responseTask = RaamenApp.HttpClient.GetAsync(new Uri(LoginServiceUri, $"?sessionToken={token}"));
            while (responseTask.Status == TaskStatus.Running)
            {
            }
            var response = responseTask.Result;

            return response.StatusCode == HttpStatusCode.OK;
        }

        private void TryFillRememberedCredentials()
        {
            var usernameCookie = Request.Cookies["raamen-username"].ToOption();
            var passwordCookie = Request.Cookies["raamen-password"].ToOption();

            usernameCookie.Map(cookie => cookie.Value)
                .Bind(username => passwordCookie.Map(cookie => cookie.Value)
                    .Map(password => (username, password)))
                .Match(
                    some: credentials =>
                    {
                        UsernameTextBox.Text = credentials.username;
                        PasswordTextBox.Text = credentials.password;
                    },
                    none: () => { });
        }
    }
}