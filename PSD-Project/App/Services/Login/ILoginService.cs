using PSD_Project.API.Features.Authentication;
using PSD_Project.App.Pages;
using Util.Try;

namespace PSD_Project.App.Services.Login
{
    public interface ILoginService
    {
        Try<UserSession, LoginError> Login(UserCredentials credentials);
    }
}