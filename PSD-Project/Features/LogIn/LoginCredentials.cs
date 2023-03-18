using System.Runtime.Serialization;

namespace PSD_Project.Features.Users
{
    [DataContract]
    public class LoginCredentials
    {
        [DataMember]
        public readonly string Username;
        [DataMember]
        public readonly string Password;

        public LoginCredentials(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}