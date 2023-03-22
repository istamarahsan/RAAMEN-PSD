using PSD_Project.API.Features.Register;
using PSD_Project.API.Features.Users;
using Util.Try;

namespace PSD_Project.App.Services.Register
{
    public interface IRegisterService
    {
        Try<User, RegisterError> RegisterNewUser(RegistrationFormDetails registrationForm);
    }
}