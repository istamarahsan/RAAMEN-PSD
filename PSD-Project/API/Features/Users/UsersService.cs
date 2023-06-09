using System;
using System.Collections.Generic;
using System.Linq;
using PSD_Project.API.Features.Register;
using Util.Option;
using Util.Try;

namespace PSD_Project.API.Features.Users
{
    public class UsersService : IUsersService, IRegisterService
    {
        private readonly IUserRepository userRepository;

        public UsersService(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public Try<List<User>, Exception> GetUsers()
        {
            return userRepository.GetUsers();
        }

        public Try<List<User>, Exception> GetUsersWithRole(int roleId)
        {
            return userRepository.GetUsersWithRole(roleId);
        }

        public Try<User, Exception> GetUserWithUsername(string username)
        {
            return userRepository.GetUsersWithUsername(username)
                .Bind(VerifyUniqueUsername);
        }

        public Try<User, Exception> CreateUser(UserDetails userDetails)
        {
            return userRepository.AddNewUser(userDetails.Username, userDetails.Email, userDetails.Password,
                userDetails.Gender, userDetails.RoleId);
        }

        public Try<User, Exception> UpdateUser(int userId, UserUpdateDetails form)
        {
            return userRepository.UpdateUser(userId, form.Username, form.Email, form.Gender);
        }

        public Try<User, Exception> GetUser(int userId)
        {
            return userRepository.GetUser(userId);
        }

        private Try<User, Exception> VerifyUniqueUsername(List<User> usersList)
        {
            return usersList.Assert(l => l.Count <= 1, _ => new Exception("Duplicate usernames"))
                .Bind(l => l.FirstOrDefault().ToOption().OrErr(() => new Exception("User not found")));
        }

        public Try<User, Exception> Register(RegistrationFormDetails registrationForm)
        {
            return userRepository.UsernameExists(registrationForm.Username)
                .Bind(userExists =>
                {
                    switch (userExists)
                    {
                        case false:
                            return userRepository.AddNewUser(
                                registrationForm.Username,
                                registrationForm.Email,
                                registrationForm.Password,
                                registrationForm.Gender,
                                0);
                        default:
                            return Try.Err<User, Exception>(new Exception("Username exists"));
                    }
                });
        }
    }
}