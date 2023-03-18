using System.Runtime.Serialization;

namespace PSD_Project.Features.LogIn
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