using System;
using System.Threading.Tasks;
using PSD_Project.API.Features.LogIn;
using PSD_Project.App.Pages;
using Util.Try;

namespace PSD_Project.Service
{
    public interface IAuthService
    {
        Task<Try<UserSessionDetails, Exception>> GetSession(int token);
        Task<Try<UserSession, Exception>> Authenticate(UserCredentials credentials);
    }
}