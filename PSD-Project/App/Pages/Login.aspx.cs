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
        private static readonly ILoginService LoginService = new LoginAuthService();
        private static readonly IAuthService AuthService = new LoginAuthService();

        protected void Page_Load(object sender, EventArgs e)
        {

            var sessionTokenCookie = Request.Cookies[Globals.SessionCookieName].ToOption();
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

            var loginTask = LoginService.Login(credentials);
            loginTask.Wait();
            loginTask.Result
                .Map(s => s.SessionToken)
                .Match(
                    ok: token =>
                    {
                        LoginResultLabel.Text = "Login successful!";
                        
                        var tokenCookie = new HttpCookie(Globals.SessionCookieName)
                        {
                            Value = token.ToString(),
                            Expires = DateTime.Now.AddDays(1)
                        };
                        Response.SetCookie(tokenCookie);

                        if (RememberMeCheckBox.Checked)
                        {
                            var usernameCookie = new HttpCookie(Globals.SavedUsernameCookieName)
                            {
                                Value = credentials.Username,
                                Expires = DateTime.Now.AddDays(1)
                            };
                            var passwordCookie = new HttpCookie(Globals.SavedPasswordCookieName)
                            {
                                Value = credentials.Password,
                                Expires = DateTime.Now.AddDays(1)
                            };
                            Response.SetCookie(passwordCookie);
                            Response.SetCookie(usernameCookie);
                        }
                        
                        Response.Redirect("Home.aspx");
                    },
                    err: exception =>
                    {
                        LoginResultLabel.Text = exception.Message;
                    });
            
        }

        private bool ValidateSession(int token)
        {
            var authTask = AuthService.Authenticate(token);
            authTask.Wait();

            return authTask.Result.IsOk();
        }

        private void TryFillRememberedCredentials()
        {
            var usernameCookie = Request.Cookies[Globals.SavedUsernameCookieName].ToOption();
            var passwordCookie = Request.Cookies[Globals.SavedPasswordCookieName].ToOption();

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