using System.Collections.Generic;
using System.Linq;
using PSD_Project.API.Features.Users;
using Util.Collections;
using Util.Option;

namespace PSD_Project.API.Features.Authentication
{
    public class UserSessionsService : IUserSessionsService
    {
        private const int MaxSessions = 100;
        private const int NoSession = -1;
        private static readonly IDictionary<int, UserSessionDetails> Sessions = new Dictionary<int, UserSessionDetails>();
        private static readonly ISet<int> TokenPool = Enumerable.Range(0, MaxSessions).ToHashSet();

        public Option<UserSessionDetails> GetSession(int sessionToken)
        {
            return Sessions.Get(sessionToken);
        }

        public Option<UserSession> CreateSessionForUser(User user)
        {
            switch (TokenPool.DefaultIfEmpty(NoSession).First())
            {
                case NoSession:
                    return Option.None<UserSession>();
                case var id:
                    TokenPool.Remove(id);
                    var details = new UserSessionDetails(user.Id, user.Username, user.Email, user.Password, user.Gender, user.Role);
                    Sessions[id] = details;
                    return Option.Some(new UserSession(id, details));
            }
        }
    }
}