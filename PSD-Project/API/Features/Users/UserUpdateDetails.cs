using System.Runtime.Serialization;

namespace PSD_Project.API.Features.Users
{
    [DataContract]
    public class UserUpdateDetails
    {
        [DataMember]
        public readonly string Username;
        [DataMember]
        public readonly string Email;
        [DataMember]
        public readonly string Gender;

        public UserUpdateDetails(string username, string email, string gender)
        {
            Username = username;
            Email = email;
            Gender = gender;
        }
    }
}