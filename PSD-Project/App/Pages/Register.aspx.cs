using System;
using System.Linq;
using System.Web.UI;
using PSD_Project.API.Features.Register;
using PSD_Project.App.Services.Register;
using Util.Option;
using Util.Try;

namespace PSD_Project.App.Pages
{
    public partial class Register : Page
    {
        private readonly IRegisterService registerService = AppServices.Singletons.RegisterService;

        protected void Page_Load(object sender, EventArgs e)
        {
            GenderRadioButtonList.SelectedIndex = 0;
        }

        protected void TextBox2_TextChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        protected void SubmitButton_OnClick(object sender, EventArgs e)
        {
            try
            {
                var form = ParseForm(
                    UsernameTextBox.Text, 
                    EmailTextBox.Text, 
                    GenderRadioButtonList.SelectedValue,
                    PasswordTextBox.Text, 
                    ConfirmPasswordTextBox.Text);
                var registerResult = registerService.RegisterNewUser(form).Unwrap();
                Response.Redirect("Login.aspx");
            }
            catch (Exception exception)
            {
                ErrorLabel.Text = exception.Message;
            }
        }

        private RegistrationFormDetails ParseForm(string username, string email, string gender, string password, string confirmPassword)
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
        private bool EndsWithDotCom(string str)
        {
            return str.EndsWith(".com");
        }
    }
}