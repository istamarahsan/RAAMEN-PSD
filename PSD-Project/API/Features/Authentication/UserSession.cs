using System.Runtime.Serialization;

namespace PSD_Project.API.Features.Authentication
{
    [DataContract]
    public class UserSession
    {
        [DataMember]
        public readonly int SessionToken;
        [DataMember]
        public readonly UserSessionDetails Details;

        public UserSession(int sessionToken, UserSessionDetails details)
        {
            SessionToken = sessionToken;
            Details = details;
        }
    }
}