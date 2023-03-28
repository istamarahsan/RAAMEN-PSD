using System;
using System.Collections.Generic;
using System.Linq;
using PSD_Project.API.Features.Profile;
using PSD_Project.API.Features.Users.Authorization;
using Util.Option;
using Util.Try;

namespace PSD_Project.API.Features.Users
{
    public class UsersService : IUsersService
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

        public Try<User, Exception> UpdateUser(int userId, UserDetails form)
        {
            return userRepository.UpdateUser(userId, form.Username, form.Email, form.Gender);
        }

        public Try<ProfileDetails, Exception> UpdateProfile(int userId, ProfileDetails form)
        {
            return GetUser(userId)
                .Bind(user => UpdateUser(userId,
                    new UserDetails(form.Username, form.Email, user.Password, form.Gender, user.Role.Id)))
                .Map(user => new ProfileDetails(user.Username, user.Email, user.Gender));
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
    }
}