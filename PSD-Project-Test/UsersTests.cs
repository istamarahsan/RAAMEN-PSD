using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Results;
using Bogus;
using Bogus.DataSets;
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

        private static readonly Faker Faker = new Faker();

        private static IEnumerable<NewUserDetails> UserDetailsGenerator()
        {
            while (true)
            {
                var username = Faker.Internet.UserName();
                var email = Faker.Internet.Email();
                var password = Faker.Internet.Password();
                var gender = Faker.PickRandom(Name.Gender.Female.ToString(), Name.Gender.Male.ToString());
                var roleId = Faker.PickRandom(TestRoles.Values).Id;
                yield return new NewUserDetails(username, email, password, gender, roleId);
            }
        }
        
        private static IEnumerable<object[]> GenerateNewUserDetails(int quantity)
        {
            return UserDetailsGenerator().Take(quantity).Select(f => new[] {f as object}).ToArray();
        }

        private class TestRepository : IUsersRepository
        {
            private readonly IDictionary<int, Role> roles;
            private readonly IDictionary<int, User> users;

            public TestRepository(IDictionary<int, Role> roles, IDictionary<int, User> users)
            {
                this.roles = roles;
                this.users = users;
            }

            public Task<Option<User>> GetUserAsync(int userId)
            {
                return Task.FromResult(users.Get(userId));
            }

            public Task<List<User>> GetUsersAsync()
            {
                return Task.FromResult(users.Values.ToList());
            }

            public Task<List<User>> GetUsersWithRoleAsync(int roleId)
            {
                return Task.FromResult(users.Values.Where(user => user.Role.Id == roleId).ToList());
            }

            public Task<Option<User>> GetUserWithUsernameAsync(string username)
            {
                return Task.FromResult(users.Values.FirstOrDefault(user => user.Username == username).ToOption());
            }

            public Task AddNewUserAsync(string username, string email, string password, string gender, int roleId)
            {
                var nextId = users.Keys.DefaultIfEmpty(0).Max() + 1;
                return roles.Get(roleId)
                    .Map(role =>
                    {
                        var user = new User(nextId, username, email, password, gender, role);
                        users[nextId] = user;
                        return Task.CompletedTask;
                    })
                    .OrElse(Task.FromException(new ArgumentException("Role with that ID does not exist")));
                
            }
        }

        [Fact]
        public async void GetAllUsersReturnsAllUsers()
        {
            var controller = new UsersController(new TestRepository(TestRoles, TestUsers));
            var users = await controller.GetAllUsers();
            Assert.True(users.OrderBy(user => user.Id).SequenceEqual(TestUsers.Values.OrderBy(user => user.Id).ToList()));
        }

        [Theory]
        [MemberData(nameof(GenerateNewUserDetails), 20)]
        public async void NewUserAddedIsRetrievable(NewUserDetails form)
        {
            var controller = new UsersController(new TestRepository(TestRoles, new Dictionary<int, User>()));
            var response = await controller.CreateNewUser(form);
            Assert.IsAssignableFrom<OkResult>(response);
            var users = await controller.GetAllUsers();
            Assert.Equal(1, users.Count);
            var user = users.FirstOrDefault();
            Assert.NotNull(user);
            Assert.Equal(form.Username, user.Username);
            Assert.Equal(form.Password, user.Password);
            Assert.Equal(form.Email, user.Email);
            Assert.Equal(form.Gender, user.Gender);
            Assert.Equal(form.RoleId, user.Role.Id);
        }
        
        [Theory]
        [MemberData(nameof(GenerateNewUserDetails), 20)]
        public async void AddingNonexistentRoleReturnsBadRequest(NewUserDetails form)
        {
            var controller = new UsersController(new TestRepository(new Dictionary<int, Role>(), new Dictionary<int, User>()));
            var response = await controller.CreateNewUser(form);
            Assert.IsAssignableFrom<BadRequestResult>(response);
        }
    }
}