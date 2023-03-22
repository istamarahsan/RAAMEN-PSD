using System.Web.Http;
using System.Web.Http.Results;
using PSD_Project.API.Features.Authentication;
using PSD_Project.App.Common;
using PSD_Project.App.Pages;
using Util.Try;

namespace PSD_Project.App.Services.Login
{
    public class AdapterLoginService : ILoginService
    {
        private readonly UserAuthenticationController userAuthenticationController = new UserAuthenticationController();
        
        public Try<UserSession, LoginError> Login(UserCredentials credentials)
        {
            return userAuthenticationController
                .Authenticate(credentials)
                .InterpretAs<UserSession>()
                .MapErr(HandleError);
        }

        private LoginError HandleError(IHttpActionResult response)
        {
            switch (response)
            {
                case BadRequestResult _:
                    return LoginError.InvalidCredentials;
                default:
                    return LoginError.InternalServiceError;
            }
        }
    }
}