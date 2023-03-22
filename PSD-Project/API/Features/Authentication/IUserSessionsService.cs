using PSD_Project.API.Features.Users;
using Util.Option;

namespace PSD_Project.API.Features.Authentication
{
    public interface IUserSessionsService
    {
        Option<UserSessionDetails> GetSession(int sessionToken);
        Option<UserSession> CreateSessionForUser(User user);
    }
}