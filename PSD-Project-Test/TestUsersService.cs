using System;
using System.Collections.Generic;
using System.Linq;
using PSD_Project.API.Features.Users;
using PSD_Project.API.Features.Users.Authorization;
using Util.Collections;
using Util.Option;
using Util.Try;

namespace PSD_Project_Test
{
    internal class TestUsersService : IUsersService
    {
        private readonly IDictionary<int, RoleDetails> roles;
        private readonly IDictionary<int, User> users;

        public TestUsersService()
        {
            roles = new Dictionary<int, RoleDetails>();
            users = new Dictionary<int, User>();
        }
        
        public TestUsersService(IDictionary<int, RoleDetails> roles, IDictionary<int, User> users)
        {
            this.roles = roles;
            this.users = users;
        }

        public Try<User, Exception> GetUser(int userId)
        {
            return users.Get(userId).OrErr(() => new Exception());
        }

        public Try<List<User>, Exception> GetUsers() => Try.Of<List<User>, Exception>(users.Values.ToList());

        public Try<List<User>, Exception> GetUsersWithRole(int roleId)
        {
            return Try.Of<List<User>, Exception>(users.Values.Where(user => user.Role.Id == roleId).ToList());
        }

        public Try<User, Exception> GetUserWithUsername(string username)
        {
            return users.Values
                .FirstOrDefault(user => user.Username == username)
                .ToOption()
                .OrErr(() => new Exception());
        }

        public Try<User, Exception> CreateUser(UserDetails userDetails) =>
            AddNewUser(
                userDetails.Username,
                userDetails.Email,
                userDetails.Password,
                userDetails.Gender,
                userDetails.RoleId);

        public Try<User, Exception> UpdateUser(int userId, UserUpdateDetails form) =>
            UpdateUser(userId, form.Username, form.Email, form.Gender);

        public Try<Role, Exception> GetRoleOfId(int roleId) => throw new NotImplementedException();

        public Try<List<User>, Exception> GetUsersWithUsername(string username)
        {
            return Try.Of<List<User>, Exception>(users.Values.Where(u => u.Username == username).ToList());
        }

        private Try<User, Exception> AddNewUser(string username, string email, string password, string gender,
            int roleId)
        {
            var nextId = users.Keys.DefaultIfEmpty(0).Max() + 1;
            var result = roles.Get(roleId)
                .Map(role =>
                {
                    var user = new User(nextId, username, email, password, gender, role);
                    users[nextId] = user;
                    return user;
                })
                .OrErr(() => new ArgumentException("Role with that ID does not exist"))
                .MapErr(err => err as Exception);
            return result;
        }

        private Try<User, Exception> UpdateUser(int userId, string username, string email, string gender)
        {
            if (!users.ContainsKey(userId))
                return Try.Err<User, Exception>(new ArgumentException("User does not exist"));

            var existingUser = users[userId];
            users[userId] = new User(userId, username, email, existingUser.Password, gender, existingUser.Role);
            return Try.Of<User, Exception>(users[userId]);
        }
    }
}