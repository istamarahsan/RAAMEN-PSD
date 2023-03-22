using System;
using System.Collections.Generic;
using System.Linq;
using PSD_Project.API.Features.Users.Authorization;
using Util.Option;
using Util.Try;

namespace PSD_Project.API.Features.Users
{
    public class UsersService : IUsersService
    {
        private readonly IUserRepository userRepository;
        private readonly IRolesRepository rolesRepository;

        public UsersService(IUserRepository userRepository, IRolesRepository rolesRepository)
        {
            this.userRepository = userRepository;
            this.rolesRepository = rolesRepository;
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
        
        public Try<bool, Exception> CanRolePlaceOrder(int roleId)
        {
            return Try.Of<bool, Exception>(roleId == 0);
        }

        public Try<bool, Exception> CanRoleHandleOrder(int roleid)
        {
            return Try.Of<bool, Exception>(roleid == 1 || roleid == 2);
        }

        public Try<Role, Exception> GetRoleOfId(int roleId)
        {
            return rolesRepository.GetRole(roleId)
                .Bind(roleDetails => ParseRoleDetails(roleDetails)
                    .OrErr(() => new Exception("Undefined role")));
        }

        private Option<Role> ParseRoleDetails(RoleDetails roleDetails)
        {
            switch (roleDetails.Name.ToLower())
            {
                case "admin":
                    return Option.Some(Role.Admin);
                case "staff":
                    return Option.Some(Role.Staff);
                case "customer":
                    return Option.Some(Role.Customer);
                default:
                    return Option.None<Role>();
            }
        }
    }
}