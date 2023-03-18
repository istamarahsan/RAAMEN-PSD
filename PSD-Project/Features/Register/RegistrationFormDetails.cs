using System.Runtime.Serialization;

namespace PSD_Project.Features.Register
{
    public partial class RegisterController
    {
        [DataContract]
        public class RegistrationFormDetails
        {
            [DataMember]
            public string Username { get; set; }
            [DataMember]
            public string Email { get; set; }
            [DataMember]
            public string Gender { get; set; }
            [DataMember]
            public string Password { get; set; }

            public RegistrationFormDetails(string username, string email, string gender, string password)
            {
                Username = username;
                Email = email;
                Gender = gender;
                Password = password;
            }
        }
    }
}