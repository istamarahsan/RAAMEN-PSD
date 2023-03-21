using PSD_Project.API.Features.Users;
using Util.Option;

namespace PSD_Project.API.Features.LogIn
{
    public interface IUserSessions
    {
        Option<UserSessionDetails> GetSession(int sessionToken);
        Option<UserSession> CreateSessionForUser(User user);
    }
}