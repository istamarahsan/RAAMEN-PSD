using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PSD_Project.App.Models;
using PSD_Project.Service;
using PSD_Project.Service.Http;
using Util.Option;
using Util.Try;

namespace PSD_Project.App.Pages
{
    public partial class Register : Page
    {
        private readonly IRegisterService registerService = Services.GetRegisterService();
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void OnSubmitButtonClicked(object sender, EventArgs e)
        {
            var usernameValidation = UsernameTextBox.Text
                .Check(
                    IsBetween5And15Characters,
                    otherwise: _ => "must be between 5 and 15 characters")
                .Bind(
                    username => username.Check(
                        IsAlphabetOrWhiteSpace,
                        otherwise: _ => "can only contain alphabets and spaces"));

            var emailValidation = EmailTextBox.Text
                .Check(
                    EndsWithDotCom,
                    otherwise: _ => "must end with '.com'");
            
            var passwordValidation = PasswordTextBox.Text.Check(
                password => password == ConfirmPasswordTextBox.Text,
                otherwise: _ => "passwords do not match");

            var genderValidation = GenderRadioButtons.SelectedItem
                .Check(
                    item => item != null,
                    otherwise: _ => "must be chosen");
            
            UsernameErrorLabel.Text = usernameValidation.Err().OrElse("");
            EmailErrorLabel.Text = emailValidation.Err().OrElse("");
            GenderErrorLabel.Text = genderValidation.Err().OrElse("");
            PasswordErrorLabel.Text = passwordValidation.Err().OrElse("");

            if (usernameValidation.IsOk()
                && emailValidation.IsOk()
                && genderValidation.IsOk()
                && passwordValidation.IsOk())
            {
                var formDetails = new RegistrationFormDetails(
                    UsernameTextBox.Text,
                    EmailTextBox.Text,
                    PasswordTextBox.Text,
                    GenderRadioButtons.SelectedItem.Value);
                var registerTask = registerService.RegisterNewUser(formDetails);
                registerTask.Wait();
                var statusCode = registerTask.Result;
                switch (statusCode)
                {
                    case HttpStatusCode.OK:
                        Response.Redirect("Login.aspx");
                        break;
                    case HttpStatusCode.Conflict:
                        UsernameErrorLabel.Text = "It seems this username already exists";
                        break;
                    default:
                        RegisterResultLabel.Text = $"Oops. Something went wrong :( - {statusCode}";
                        break;
                }
            }
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