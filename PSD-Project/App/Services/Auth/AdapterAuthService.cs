using PSD_Project.API.Features.Authentication;
using PSD_Project.App.Common;
using Util.Try;

namespace PSD_Project.App.Services.Auth
{
    public class AdapterAuthService : IAuthService
    {
        private readonly UserAuthenticationController authenticationController = new UserAuthenticationController(); 
        
        public Try<UserSessionDetails, AuthError> GetSession(int token)
        {
            return authenticationController.GetSession(token)
                .InterpretAs<UserSessionDetails>()
                .MapErr(_ => AuthError.InvalidSession);
        }
    }
}