using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PSD_Project;
using PSD_Project.Features;
using PSD_Project.Features.Users;
using Util.Option;
using Util.Collections;
using Xunit;

namespace PSD_Project_Test
{
    public class UsersTests
    {
        private static readonly IDictionary<int, Role> TestRoles = Enumerable.Range(0, 5)
            .Select(i => new Role(i, $"Role {i}"))
            .ToDictionary(role => role.Id);

        private static readonly IDictionary<int, User> TestUsers = new List<User>
        {
            new User(
                0,
                "Bob",
                "bob@mail.com",
                "bob2",
                "m",
                TestRoles[0]),
            new User(
                1,
                "Alice",
                "alice@mail.com",
                "alice3",
                "f",
                TestRoles[1])
        }.ToDictionary(user => user.Id);

        private class TestRepository : IUsersRepository
        {
            public Task<Option<User>> GetUserAsync(int userId)
            {
                return Task.FromResult(TestUsers.Get(userId));
            }

            public Task<List<User>> GetUsersAsync()
            {
                return Task.FromResult(TestUsers.Values.ToList());
            }

            public Task<List<User>> GetUsersWithRoleAsync(int roleId)
            {
                return Task.FromResult(TestUsers.Values.Where(user => user.Role.Id == roleId).ToList());
            }

            public Task<Option<User>> GetUserWithUsernameAsync(string username)
            {
                return Task.FromResult(TestUsers.Values.FirstOrDefault(user => user.Username == username).ToOption());
            }

            public Task AddNewUserAsync(string username, string email, string password, string gender, int roleId)
            {
                var nextId = TestUsers.Keys.Max() + 1;
                return TestRoles.Get(roleId)
                    .Map(role =>
                    {
                        var user = new User(nextId, username, email, password, gender, role);
                        TestUsers[nextId] = user;
                        return Task.CompletedTask;
                    })
                    .OrElse(Task.FromException(new Exception("Role with that ID does not exist")));
                
            }
        }

        [Fact]
        public async void GetAllUsersReturnsAllUsers()
        {
            var controller = new UsersController(new TestRepository());
            var users = await controller.GetAllUsers();
            Assert.True(users.OrderBy(user => user.Id).SequenceEqual(TestUsers.Values.OrderBy(user => user.Id).ToList()));
        }
    }
}