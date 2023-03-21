using System;
using System.Threading.Tasks;
using PSD_Project.API.Features.LogIn;
using PSD_Project.App.Pages;
using Util.Try;

namespace PSD_Project.Services
{
    public interface ILoginService
    {
        Task<Try<UserSession, Exception>> Login(UserCredentials credentials);
    }
}