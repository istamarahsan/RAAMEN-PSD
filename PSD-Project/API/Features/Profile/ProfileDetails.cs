using System.Runtime.Serialization;

namespace PSD_Project.API.Features.Profile
{
    [DataContract]
    public class ProfileDetails
    {
        [DataMember]
        public readonly string Username;
        [DataMember]
        public readonly string Email;
        [DataMember]
        public readonly string Gender;

        public ProfileDetails(string username, string email, string gender)
        {
            Username = username;
            Email = email;
            Gender = gender;
        }
    }
}