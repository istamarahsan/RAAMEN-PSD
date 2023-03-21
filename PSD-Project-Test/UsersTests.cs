using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Results;
using Bogus;
using Bogus.DataSets;
using PSD_Project;
using PSD_Project.API.Features.Users;
using PSD_Project.App.Models;
using Util.Option;
using Util.Collections;
using Util.Try;
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
        
        public static IEnumerable<object[]> GenerateNewUserDetails(int quantity, int numberAtATime)
        {
            return Enumerable.Range(0, quantity)
                .Select(_ => UserDetailsGenerator().Take(numberAtATime).Cast<object>().ToArray()).ToList();
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

            public Task<Option<User>> GetUser(int userId)
            {
                return Task.FromResult(users.Get(userId));
            }

            public Task<List<User>> GetUsers()
            {
                return Task.FromResult(users.Values.ToList());
            }

            public Task<List<User>> GetUsersWithRole(int roleId)
            {
                return Task.FromResult(users.Values.Where(user => user.Role.Id == roleId).ToList());
            }

            public Task<Option<User>> GetUserWithUsername(string username)
            {
                return Task.FromResult(users.Values.FirstOrDefault(user => user.Username == username).ToOption());
            }

            public Task<Try<User, Exception>> AddNewUser(string username, string email, string password, string gender, int roleId)
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
                return Task.FromResult(result);
            }

            public Task<Try<User, Exception>> UpdateUser(int userId, string username, string email, string gender)
            {
                if (!users.ContainsKey(userId)) return Task.FromResult(Try.Err<User, Exception>(new ArgumentException("User does not exist"))) ;

                var existingUser = users[userId];
                users[userId] = new User(userId, username, email, existingUser.Password, gender, existingUser.Role);
                return Task.FromResult(Try.Of<User, Exception>(users[userId]));
            }
        }

        [Fact]
        public async void GetAllUsersReturnsAllUsers()
        {
            var generatedUsers = UserDetailsGenerator()
                .Take(20)
                .Select((gen, i) =>
                    new User(i, gen.Username, gen.Email, gen.Password, gen.Gender, TestRoles[gen.RoleId]))
                .ToList();
            var controller = new UsersController(new TestRepository(TestRoles, generatedUsers.ToDictionary(u => u.Id)));
            var users = await controller.GetAllUsers();
            Assert.True(users.OrderBy(user => user.Id).SequenceEqual(generatedUsers.OrderBy(user => user.Id).ToList()));
        }

        [Theory]
        [MemberData(nameof(GenerateNewUserDetails), 20, 1)]
        public async void NewUserAddedIsRetrievable(NewUserDetails form)
        {
            var controller = new UsersController(new TestRepository(TestRoles, new Dictionary<int, User>()));
            var response = await controller.CreateNewUser(form);
            Assert.IsAssignableFrom<OkNegotiatedContentResult<User>>(response);
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
        [MemberData(nameof(GenerateNewUserDetails), 20, 1)]
        public async void TryingToAddUserWithNonexistentRoleReturnsBadRequest(NewUserDetails form)
        {
            var controller = new UsersController(new TestRepository(new Dictionary<int, Role>(), new Dictionary<int, User>()));
            var response = await controller.CreateNewUser(form);
            Assert.IsAssignableFrom<BadRequestResult>(response);
        }

        [Theory]
        [MemberData(nameof(GenerateNewUserDetails), 20, 1)]
        public async void CreatingUserReturnsUser(NewUserDetails form)
        {
            var controller = new UsersController(new TestRepository(TestRoles, new Dictionary<int, User>()));
            var response = await controller.CreateNewUser(form);
            Assert.IsAssignableFrom<OkNegotiatedContentResult<User>>(response);
            var returnedUser = ((OkNegotiatedContentResult<User>)response).Content;
            Assert.Equal(form.Username, returnedUser.Username);
            Assert.Equal(form.Email, returnedUser.Email);
            Assert.Equal(form.Password, returnedUser.Password);
            Assert.Equal(form.RoleId, returnedUser.Role.Id);
        }
        
        [Theory]
        [MemberData(nameof(GenerateNewUserDetails), 20, 1)]
        public async void CreatingUserStoresUser(NewUserDetails form)
        {
            var usersStorage = new Dictionary<int, User>();
            var controller = new UsersController(new TestRepository(TestRoles, usersStorage));
            await controller.CreateNewUser(form);
            Assert.Equal(1, usersStorage.Count);
            Assert.Contains(usersStorage.Values, user => user.Username == form.Username
                                                         && user.Email == form.Email
                                                         && user.Password == form.Password
                                                         && user.Role.Id == form.RoleId);
        }

        [Theory]
        [MemberData(nameof(GenerateNewUserDetails), 20, 2)]
        public async void UpdatingUserUpdatesStoredUser(NewUserDetails initialDetails, NewUserDetails newDetails)
        {
            var usersStorage = new Dictionary<int, User>
            {
                [0] = new User(0, initialDetails.Username, initialDetails.Email, initialDetails.Password, initialDetails.Gender, TestRoles[initialDetails.RoleId])
            };
            var controller = new UsersController(new TestRepository(TestRoles, usersStorage));
            await controller.UpdateUser(0, new UserUpdateDetails(newDetails.Username, newDetails.Email, newDetails.Gender));
            Assert.Equal(1, usersStorage.Count);
            Assert.Contains(usersStorage.Values, user => user.Username == newDetails.Username 
                                                         && user.Email == newDetails.Email 
                                                         && user.Gender == newDetails.Gender);
        }
        
        [Theory]
        [MemberData(nameof(GenerateNewUserDetails), 20, 2)]
        public async void UpdatingUserReturnsUpdatedUser(NewUserDetails initialDetails, NewUserDetails newDetails)
        {
            var usersStorage = new Dictionary<int, User>
            {
                [0] = new User(0, initialDetails.Username, initialDetails.Email, initialDetails.Password, initialDetails.Gender, TestRoles[initialDetails.RoleId])
            };
            var controller = new UsersController(new TestRepository(TestRoles, usersStorage));
            var response = await controller.UpdateUser(0, new UserUpdateDetails(newDetails.Username, newDetails.Email, newDetails.Gender));
            var returnedUser = ((OkNegotiatedContentResult<User>)response).Content;
            Assert.Equal(0, returnedUser.Id);
            Assert.Equal(initialDetails.Password, returnedUser.Password);
            Assert.Equal(newDetails.Username, returnedUser.Username);
            Assert.Equal(newDetails.Email, returnedUser.Email);
            Assert.Equal(newDetails.Gender, returnedUser.Gender);
        }
    }
}