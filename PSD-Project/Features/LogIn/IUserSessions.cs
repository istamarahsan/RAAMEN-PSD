using PSD_Project.Features.Users;
using Util.Option;

namespace PSD_Project.Features.LogIn
{
    public interface IUserSessions
    {
        Option<UserSessionDetails> GetSession(int sessionToken);
        Option<UserSession> CreateSessionForUser(User user);
    }
}