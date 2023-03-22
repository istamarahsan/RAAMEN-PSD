using System;
using PSD_Project.App.Pages;
using Util.Try;

namespace PSD_Project.API.Features.Authentication
{
    public interface IAuthenticationService
    {
        Try<UserSessionDetails, Exception> GetSession(int token);
        Try<UserSession, Exception> Authenticate(UserCredentials credentials);
    }
}