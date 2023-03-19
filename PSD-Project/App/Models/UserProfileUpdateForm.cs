using System.Runtime.Serialization;

namespace PSD_Project.App.Models
{
    [DataContract]
    public class UserProfileUpdateForm
    {
        [DataMember]
        public readonly string NewUsername;
        [DataMember]
        public readonly string NewEmail;
        [DataMember]
        public readonly string NewGender;

        public UserProfileUpdateForm(string newUsername, string newEmail, string newGender)
        {
            NewUsername = newUsername;
            NewEmail = newEmail;
            NewGender = newGender;
        }
    }
}