using System.Runtime.Serialization;

namespace PSD_Project.App.Pages
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