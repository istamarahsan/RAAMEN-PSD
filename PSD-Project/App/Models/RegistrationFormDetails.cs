using System.Runtime.Serialization;

namespace PSD_Project.App.Models
{ 
    [DataContract]
    public class RegistrationFormDetails
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
}