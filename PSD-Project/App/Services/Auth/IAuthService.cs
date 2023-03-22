using PSD_Project.API.Features.Authentication;
using Util.Try;

namespace PSD_Project.App.Services.Auth
{
    public interface IAuthService
    {
        Try<UserSessionDetails, AuthError> GetSession(int token);
    }
}