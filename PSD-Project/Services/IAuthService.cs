using System;
using System.Threading.Tasks;
using PSD_Project.Features.LogIn;
using Util.Try;

namespace PSD_Project.Services
{
    public interface IAuthService
    {
        Task<Try<UserSessionDetails, Exception>> Authenticate(int token);
    }
}