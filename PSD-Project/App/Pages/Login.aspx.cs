using System;
using System.Web;
using System.Web.UI;
using PSD_Project.API.Features.Authentication;
using PSD_Project.App.Common;
using PSD_Project.App.Services.Login;
using PSD_Project.App.Services.Auth;
using Util.Option;
using Util.Try;

namespace PSD_Project.App.Pages
{
    public partial class Login : Page
    {
        private readonly ILoginService loginService = AppServices.Singletons.LoginService;
        private readonly IAuthService authService = AppServices.Singletons.AuthService;
        protected void Page_Load(object sender, EventArgs e)
        {
            Request.GetTokenFromCookie()
                .Bind(token => authService.GetSession(token).Ok())
                .Match(
                    some: sessionDetails =>
                    {
                        Session[Globals.SavedSessionName] = sessionDetails;
                        Response.Redirect("Home.aspx");
                    },
                    none: TryFillRememberedCredentials);
        }

        protected void OnClick(object sender, EventArgs e)
        {
            var credentials = new UserCredentials(
                UsernameTextBox.Text,
                PasswordTextBox.Text);

            loginService.Login(credentials)
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
                    err: error =>
                    {
                        switch (error)
                        {
                            case LoginError.InvalidCredentials:
                                // display some error stuff
                                break;
                            case LoginError.InternalServiceError:
                            default:
                                // display some error stuff
                                break;
                        }
                    });
            
        }

        private void TryFillRememberedCredentials()
        {
            Request.GetUsernameFromCookie()
                .Bind(username => Request.GetPasswordFromCookie()
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