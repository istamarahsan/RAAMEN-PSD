using System.Runtime.Serialization;

namespace PSD_Project.API.Features.Users
{
    [DataContract]
    public class User
    {
        [DataMember]
        public readonly int Id;
        [DataMember]
        public readonly string Username;
        [DataMember]
        public readonly string Email;
        [DataMember] 
        public readonly string Password;
        [DataMember]
        public readonly string Gender;
        [DataMember(Name = "Role")] 
        public readonly RoleDetails Role;

        public User(int id, string username, string email, string password, string gender, RoleDetails role)
        {
            Id = id;
            Username = username;
            Email = email;
            Password = password;
            Gender = gender;
            Role = role;
        }
    }
}