using PSD_Project.API.Features.Register;
using PSD_Project.API.Features.Users;
using PSD_Project.App.Common;
using Util.Try;

namespace PSD_Project.App.Services.Register
{
    public class AdapterRegisterService : IRegisterService
    {
        private readonly RegisterController registerController = new RegisterController();
        
        public Try<User, RegisterError> RegisterNewUser(RegistrationFormDetails registrationForm)
        {
            return registerController.Register(registrationForm)
                .InterpretAs<User>()
                .MapErr(_ => RegisterError.InternalServiceError);
        }
    }
}