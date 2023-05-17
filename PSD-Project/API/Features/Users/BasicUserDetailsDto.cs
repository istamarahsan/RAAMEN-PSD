using System.Runtime.Serialization;

namespace PSD_Project.API.Features.Users
{
    [DataContract]
    public class BasicUserDetailsDto
    {
        [DataMember]
        public int Id;
        [DataMember] 
        public string Username;
    }
}