using System;
using PSD_Project.API.Features.Users;
using PSD_Project.App.Models;
using Util.Try;

namespace PSD_Project.API.Features.Register
{
    public interface IRegisterService
    {
        Try<User, Exception> RegisterNewUser(RegistrationFormDetails form);
    }
}