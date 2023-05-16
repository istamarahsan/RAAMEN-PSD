using System;
using PSD_Project.API.Features.Users;
using Util.Try;

namespace PSD_Project.API.Features.Register
{
    public interface IRegisterService
    {
        Try<User, Exception> Register(RegistrationFormDetails registrationFormDetails);
    }
}