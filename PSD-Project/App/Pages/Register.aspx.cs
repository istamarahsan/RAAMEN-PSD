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
using Util.Option;
using Util.Try;

namespace PSD_Project.App.Pages
{
    public partial class Register : Page
    {
        [DataContract]
        private class RegistrationFormDetails
        {
            [DataMember]
            public readonly string Username;
            [DataMember]
            public readonly string Email;
            [DataMember]
            public readonly string Password;
            [DataMember]
            public readonly string Gender;

            public RegistrationFormDetails(string username, string email, string password, string gender)
            {
                Username = username;
                Email = email;
                Password = password;
                Gender = gender;
            }
        }
        
        private static readonly Uri UsersServiceUri = new Uri("http://localhost:5000/api/register");
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
                var json = JsonConvert.SerializeObject(formDetails, Formatting.None);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var postTask = RaamenApp.HttpClient.PostAsync(UsersServiceUri, content);
                postTask.Wait();
                var post = postTask.Result;
                switch (post.StatusCode)
                {
                    case HttpStatusCode.OK:
                        RegisterResultLabel.Text = "User Successfully Created";
                        break;
                    case HttpStatusCode.Conflict:
                        UsernameErrorLabel.Text = "It seems this username already exists";
                        break;
                    default:
                        RegisterResultLabel.Text = $"Oops. Something went wrong :( - {post.StatusCode}";
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