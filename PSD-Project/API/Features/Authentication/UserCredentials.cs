using System.Runtime.Serialization;

namespace PSD_Project.API.Features.Authentication
{
    [DataContract]
    public class UserCredentials
    {
        [DataMember]
        public readonly string Username;
        [DataMember]
        public readonly string Password;

        public UserCredentials(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}