using System;
using PSD_Project.API.Features.Users;
using PSD_Project.App.Models;
using Util.Try;

namespace PSD_Project.API.Features.Register
{
    public class RegisterService : IRegisterService
    {
        private readonly IUsersService usersService;

        public RegisterService(IUsersService usersService)
        {
            this.usersService = usersService;
        }

        public Try<User, Exception> RegisterNewUser(RegistrationFormDetails form)
        {
            return usersService.GetUserWithUsername(form.Username)
                // assume that error means username is not taken
                .Err()
                .OrErr(() => new Exception("Username is taken"))
                .Map(_ => new UserDetails(form.Username, form.Email, form.Password, form.Gender, 0))
                .Bind(usersService.CreateUser);
        }
    }
}