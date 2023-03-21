using System;
using System.Linq;
using System.Net;
using System.Web.UI;
using PSD_Project.API.Features.Register;
using PSD_Project.API.Service;
using PSD_Project.App.Models;
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
                .Assert(
                    IsBetween5And15Characters,
                    otherwise: _ => "must be between 5 and 15 characters")
                .Bind(
                    username => username.Assert(
                        IsAlphabetOrWhiteSpace,
                        otherwise: _ => "can only contain alphabets and spaces"));

            var emailValidation = EmailTextBox.Text
                .Assert(
                    EndsWithDotCom,
                    otherwise: _ => "must end with '.com'");
            
            var passwordValidation = PasswordTextBox.Text.Assert(
                password => password == ConfirmPasswordTextBox.Text,
                otherwise: _ => "passwords do not match");

            var genderValidation = GenderRadioButtons.SelectedItem
                .Assert(
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
                registerService.RegisterNewUser(formDetails)
                    .Match(
                        ok: _ => Response.Redirect("Login.aspx"),
                        err: _ => RegisterResultLabel.Text = $"Oops. Something went wrong :(");
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