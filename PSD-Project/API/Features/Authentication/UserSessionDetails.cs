using System.Runtime.Serialization;
using PSD_Project.API.Features.Users;

namespace PSD_Project.API.Features.Authentication
{
    [DataContract]
    public class UserSessionDetails
    {
        [DataMember]
        public readonly int Id;
        [DataMember]
        public readonly string Username;
        [DataMember]
        public readonly string Email;
        [DataMember]
        public readonly string Gender;
        [DataMember] 
        public readonly RoleDetails Role;

        public UserSessionDetails(int id, string username, string email, string password, string gender, RoleDetails role)
        {
            Id = id;
            Username = username;
            Email = email;
            Gender = gender;
            Role = role;
        }
    }
}