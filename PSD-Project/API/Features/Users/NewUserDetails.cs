using System.Runtime.Serialization;

namespace PSD_Project.API.Features.Users
{
    [DataContract]
    public class NewUserDetails
    {
        [DataMember]
        public readonly string Username;
        [DataMember]
        public readonly string Email;
        [DataMember]
        public readonly string Gender;
        [DataMember]
        public readonly string Password;
        [DataMember]
        public readonly int RoleId;

        public NewUserDetails(string username, string email, string password, string gender, int roleId)
        {
            Username = username;
            Email = email;
            Gender = gender;
            Password = password;
            RoleId = roleId;
        }
    }
}