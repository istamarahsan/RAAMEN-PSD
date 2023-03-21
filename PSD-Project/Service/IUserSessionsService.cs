using PSD_Project.API.Features.LogIn;
using PSD_Project.API.Features.Users;
using Util.Option;

namespace PSD_Project.Service
{
    public interface IUserSessionsService
    {
        Option<UserSessionDetails> GetSession(int sessionToken);
        Option<UserSession> CreateSessionForUser(User user);
    }
}