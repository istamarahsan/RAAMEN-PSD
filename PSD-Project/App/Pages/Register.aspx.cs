using System;
using System.Linq;
using System.Web.UI;
using PSD_Project.API.Features.Register;
using PSD_Project.App.Common;
using PSD_Project.App.Services.Register;
using Util.Option;
using Util.Try;
using IRegisterService = PSD_Project.App.Services.Register.IRegisterService;

namespace PSD_Project.App.Pages
{
    public partial class Register : Page
    {
        private readonly IRegisterService registerService = AppServices.Singletons.RegisterService;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;
            if (Session.GetUserSession().IsSome())
            {
                Response.Redirect("Home.aspx");
            }
            GenderRadioButtonList.SelectedIndex = 0;
        }

        protected void SubmitButton_OnClick(object sender, EventArgs e)
        {
            try
            {
                var form = ParseForm(
                    UsernameTextBox.Text, 
                    EmailTextBox.Text, 
                    GenderRadioButtonList.SelectedValue.Trim(' ', '\n'),
                    PasswordTextBox.Text, 
                    ConfirmPasswordTextBox.Text);
                var registerResult = registerService.RegisterNewUser(form);
                registerResult.Match(
                    ok: _ =>
                    {
                        Response.Redirect("Login.aspx");
                    },
                    err: error =>
                    {
                        switch (error)
                        {
                            case RegisterError.UsernameTaken:
                                ErrorLabel.Text = "That username is already taken.";
                                break;
                            case RegisterError.InternalServiceError:
                                ErrorLabel.Text = "Something went wrong. Please try again later.";
                                break;
                        }
                    });
            }
            catch (ArgumentException exception)
            {
                ErrorLabel.Text = exception.Message;
            }
        }

        private RegistrationFormDetails ParseForm(string username, string email, string gender, string password,
            string confirmPassword)
        {
            if (!(username.Length >= 5 && username.Length <= 15))
            {
                throw new ArgumentException("Username must be between 5 and 15 characters");
            }

            if (!username.All(@char => char.IsLetter(@char) || char.IsWhiteSpace(@char)))
            {
                throw new ArgumentException("Username must be only alphabet or whitespace");
            }

            if (!email.EndsWith(".com"))
            {
                throw new ArgumentException("Email must end with '.com'");
            }

            if (password != confirmPassword)
            {
                throw new ArgumentException("Passwords do not match.");
            }

            return new RegistrationFormDetails(
                username,
                email,
                gender,
                password);
        }
    }
}