using PSD_Project.API.Features.Authentication;
using Util.Try;

namespace PSD_Project.App.Services.Login
{
    public enum LoginError
    {
        InternalServiceError,
        InvalidCredentials
    }
}