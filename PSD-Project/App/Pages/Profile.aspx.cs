using System;
using System.Linq;
using System.Net;
using System.Web.UI;
using PSD_Project.API.Features.Authentication;
using PSD_Project.API.Features.Users;
using PSD_Project.API.Service;
using PSD_Project.App.Common;
using Util.Option;
using Util.Try;

namespace PSD_Project.App.Pages
{
    public partial class Profile : Page
    {
        private static readonly IAuthenticationService AuthenticationService = Services.GetAuthenticationService();
        private static readonly IUsersService UsersService = Services.GetUsersService();
        private Option<UserSessionDetails> userSession = Option.None<UserSessionDetails>();

        protected void Page_Load(object sender, EventArgs e)
        {
            userSession = Session.GetUserSession();
            if (userSession.IsSome())
                return;

            Request.Cookies[Globals.SessionCookieName].ToOption()
                .Map(cookie => cookie.Value)
                .Bind(str => str.TryParseInt().Ok())
                .Bind(token => AuthenticationService.GetSession(token).Ok())
                .Match(
                    details => Session[Globals.SavedSessionName] = details,
                    () => Response.Redirect("Login.aspx"));
        }

        protected void OnSubmitButtonClicked(object sender, EventArgs e)
        {
            var usernameValidation = UsernameTextBox.Text
                .Assert(
                    IsBetween5And15Characters,
                    _ => "must be between 5 and 15 characters")
                .Bind(
                    username => username.Assert(
                        IsAlphabetOrWhiteSpace,
                        _ => "can only contain alphabets and spaces"));

            var emailValidation = EmailTextBox.Text
                .Assert(
                    EndsWithDotCom,
                    _ => "must end with '.com'");

            var genderValidation = GenderRadioButtons.SelectedItem
                .Assert(
                    item => item != null,
                    _ => "must be chosen")
                .Map(item => item.Text);

            UsernameErrorLabel.Text = usernameValidation.Err().OrElse("");
            EmailErrorLabel.Text = emailValidation.Err().OrElse("");
            GenderErrorLabel.Text = genderValidation.Err().OrElse("");

            if (usernameValidation.IsErr()
                || emailValidation.IsErr()
                || genderValidation.IsErr())
                return;

            var passwordValidation = userSession
                .Map(session => new UserCredentials(session.Username, PasswordTextBox.Text))
                .Bind(credentials =>  AuthenticationService.Authenticate(credentials).Ok())
                .OrErr(() => "please check your password and try again");

            PasswordErrorLabel.Text = passwordValidation.Err().OrElse("");

            if (passwordValidation.IsErr()) return;

            usernameValidation.Ok()
                .Bind(username =>
                    emailValidation.Ok()
                        .Map(email => (username, email)))
                .Bind(tuple =>
                    genderValidation.Ok()
                        .Map(gender => new UserUpdateDetails(tuple.username, tuple.email, gender)))
                .Bind(form => userSession.Map(s => s.Id).Map(id => (id, form)))
                .Bind(tuple => UsersService.UpdateUser(tuple.id, tuple.form).Ok())
                .Match(
                    user => UpdateProfileResultLabel.Text = "Profile Updated!",
                    () => UpdateProfileResultLabel.Text = $"Error");
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