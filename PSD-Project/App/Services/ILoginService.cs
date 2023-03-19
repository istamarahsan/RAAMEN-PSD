using System;
using System.Threading.Tasks;
using PSD_Project.App.Pages;
using PSD_Project.Features.LogIn;
using Util.Try;

namespace PSD_Project.App
{
    public interface ILoginService
    {
        Task<Try<UserSession, Exception>> Login(UserCredentials credentials);
    }
}