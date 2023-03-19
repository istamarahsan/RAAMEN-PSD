using System;
using System.Threading.Tasks;
using PSD_Project.Features.LogIn;
using Util.Try;

namespace PSD_Project.App
{
    public interface IAuthService
    {
        Task<Try<UserSessionDetails, Exception>> Authenticate(int token);
    }
}