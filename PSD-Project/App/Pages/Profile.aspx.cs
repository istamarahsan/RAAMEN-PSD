using System;
using System.Linq;
using System.Net;
using System.Web.UI;
using PSD_Project.App.Common;
using PSD_Project.App.Models;
using PSD_Project.Features.LogIn;
using Util.Option;
using Util.Try;

namespace PSD_Project.App.Pages
{
    public partial class Profile : Page
    {
        private static readonly IAuthService AuthService = new LoginAuthService();
        private static readonly ILoginService LoginService = new LoginAuthService();
        private static readonly IUsersService UsersService = new UsersService();
        private Option<UserSessionDetails> userSession = Option.None<UserSessionDetails>();

        protected void Page_Load(object sender, EventArgs e)
        {
            userSession = Request.Cookies[Globals.SessionCookieName].ToOption()
                .Map(cookie => cookie.Value)
                .Bind(str => str.TryParseInt().Ok())
                .Bind(token =>
                {
                    var authTask = AuthService.Authenticate(token);
                    authTask.Wait();
                    return authTask.Result.Ok();
                });

            if (userSession.IsNone()) Response.Redirect("Login.aspx");
        }

        protected void OnSubmitButtonClicked(object sender, EventArgs e)
        {
            var usernameValidation = UsernameTextBox.Text
                .Check(
                    IsBetween5And15Characters,
                    _ => "must be between 5 and 15 characters")
                .Bind(
                    username => username.Check(
                        IsAlphabetOrWhiteSpace,
                        _ => "can only contain alphabets and spaces"));

            var emailValidation = EmailTextBox.Text
                .Check(
                    EndsWithDotCom,
                    _ => "must end with '.com'");

            var genderValidation = GenderRadioButtons.SelectedItem
                .Check(
                    item => item != null,
                    _ => "must be chosen")
                .Map(item => item.Text);

            if (usernameValidation.IsErr()
                || emailValidation.IsErr()
                || genderValidation.IsErr())
            {
                UsernameErrorLabel.Text = usernameValidation.Err().OrElse("");
                EmailErrorLabel.Text = emailValidation.Err().OrElse("");
                GenderErrorLabel.Text = genderValidation.Err().OrElse("");
                return;
            }

            var passwordValidation = userSession
                .Map(session => new UserCredentials(session.Username, PasswordTextBox.Text))
                .Bind(credentials =>
                {
                    var authTask = LoginService.Login(credentials);
                    authTask.Wait();
                    return authTask.Result.Ok();
                })
                .OrErr(() => "please check your password and try again");

            if (passwordValidation.IsErr()) PasswordErrorLabel.Text = passwordValidation.Err().OrElse("");

            usernameValidation.Ok()
                .Bind(username =>
                    emailValidation.Ok()
                        .Map(email => (username, email)))
                .Bind(tuple =>
                    genderValidation.Ok()
                        .Map(gender => new UserProfileUpdateForm(tuple.username, tuple.email, gender)))
                .Map(form =>
                {
                    var updateTask = UsersService.TryUpdateUser(form);
                    updateTask.Wait();
                    return updateTask.Result;
                })
                .Match(
                    statusCode =>
                    {
                        switch (statusCode)
                        {
                            case HttpStatusCode.OK:
                                UpdateProfileResultLabel.Text = "Profile Updated!";
                                break;
                            default:
                                UpdateProfileResultLabel.Text = $"Error: {statusCode}";
                                break;
                        }
                    },
                    () => { });
        }

        private bool IsBetween5And15Characters(string str)
        {
            return str.Length >= 5 && str.Length <= 15;
        }

        private bool IsAlphabetOrWhiteSpace(string str)
        {
            return str.All(@char => char.IsLetter(@char) || char.IsWhiteSpace(@char));
        }

        private bool EndsWithDotCom(string str)
        {
            return str.EndsWith(".com");
        }
    }
}