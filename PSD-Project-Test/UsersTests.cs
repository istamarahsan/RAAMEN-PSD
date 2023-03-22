using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Results;
using Bogus;
using Bogus.DataSets;
using PSD_Project.API.Features.Users;
using Xunit;

namespace PSD_Project_Test
{
    public class UsersTests
    {
        private static readonly IDictionary<int, RoleDetails> TestRoles = Enumerable.Range(0, 5)
            .Select(i => new RoleDetails(i, $"Role {i}"))
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

        private static IEnumerable<UserDetails> UserDetailsGenerator()
        {
            while (true)
            {
                var username = Faker.Internet.UserName();
                var email = Faker.Internet.Email();
                var password = Faker.Internet.Password();
                var gender = Faker.PickRandom(Name.Gender.Female.ToString(), Name.Gender.Male.ToString());
                var roleId = Faker.PickRandom(TestRoles.Values).Id;
                yield return new UserDetails(username, email, password, gender, roleId);
            }
        }
        
        public static IEnumerable<object[]> GenerateNewUserDetails(int quantity, int numberAtATime)
        {
            return Enumerable.Range(0, quantity)
                .Select(_ => UserDetailsGenerator().Take(numberAtATime).Cast<object>().ToArray()).ToList();
        }

        [Fact]
        public void GetAllUsersReturnsAllUsers()
        {
            var generatedUsers = UserDetailsGenerator()
                .Take(20)
                .Select((gen, i) =>
                    new User(i, gen.Username, gen.Email, gen.Password, gen.Gender, TestRoles[gen.RoleId]))
                .ToList();
            var usersService = new TestUsersService(TestRoles, generatedUsers.ToDictionary(u => u.Id));
            var authService = new TestAuthService(usersService);
            var controller = new UsersController(usersService, authService, authService);
            var users = ((OkNegotiatedContentResult<List<User>>)controller.GetAllUsers()).Content;
            Assert.True(users.OrderBy(user => user.Id).SequenceEqual(generatedUsers.OrderBy(user => user.Id).ToList()));
        }

        [Theory]
        [MemberData(nameof(GenerateNewUserDetails), 20, 1)]
        public void NewUserAddedIsRetrievable(UserDetails form)
        {
            var usersService = new TestUsersService(TestRoles, new Dictionary<int, User>());
            var authService = new TestAuthService(usersService);
            var controller = new UsersController(usersService, authService, authService);
            var response = controller.CreateNewUser(form);
            Assert.IsAssignableFrom<OkNegotiatedContentResult<User>>(response);
            var users = ((OkNegotiatedContentResult<List<User>>)controller.GetAllUsers()).Content;
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
        public void TryingToAddUserWithNonexistentRoleReturnsBadRequest(UserDetails form)
        {
            var users = new TestUsersService();
            var auth = new TestAuthService(users);
            var controller = new UsersController(users, auth, auth);
            var response = controller.CreateNewUser(form);
            Assert.IsAssignableFrom<BadRequestResult>(response);
        }

        [Theory]
        [MemberData(nameof(GenerateNewUserDetails), 20, 1)]
        public void CreatingUserReturnsUser(UserDetails form)
        {
            var usersService = new TestUsersService(TestRoles, new Dictionary<int, User>());
            var authService = new TestAuthService(usersService);
            var controller = new UsersController(usersService, authService, authService);
            var response = controller.CreateNewUser(form);
            Assert.IsAssignableFrom<OkNegotiatedContentResult<User>>(response);
            var returnedUser = ((OkNegotiatedContentResult<User>)response).Content;
            Assert.Equal(form.Username, returnedUser.Username);
            Assert.Equal(form.Email, returnedUser.Email);
            Assert.Equal(form.Password, returnedUser.Password);
            Assert.Equal(form.RoleId, returnedUser.Role.Id);
        }
        
        [Theory]
        [MemberData(nameof(GenerateNewUserDetails), 20, 1)]
        public void CreatingUserStoresUser(UserDetails form)
        {
            var usersStorage = new Dictionary<int, User>();
            var usersService = new TestUsersService(TestRoles, usersStorage);
            var authService = new TestAuthService(usersService);
            var controller = new UsersController(usersService, authService, authService);
            controller.CreateNewUser(form);
            Assert.Equal(1, usersStorage.Count);
            Assert.Contains(usersStorage.Values, user => user.Username == form.Username
                                                         && user.Email == form.Email
                                                         && user.Password == form.Password
                                                         && user.Role.Id == form.RoleId);
        }

        [Theory]
        [MemberData(nameof(GenerateNewUserDetails), 20, 2)]
        public void UpdatingUserUpdatesStoredUser(UserDetails initialDetails, UserDetails details)
        {
            var usersStorage = new Dictionary<int, User>
            {
                [0] = new User(0, initialDetails.Username, initialDetails.Email, initialDetails.Password, initialDetails.Gender, TestRoles[initialDetails.RoleId])
            };
            var users = new TestUsersService(TestRoles, usersStorage);
            var auth = new TestAuthService(users);
            var controller = new UsersController(users, auth, auth);
            controller.UpdateUser(0, new UserUpdateDetails(details.Username, details.Email, details.Gender));
            Assert.Equal(1, usersStorage.Count);
            Assert.Contains(usersStorage.Values, user => user.Username == details.Username 
                                                         && user.Email == details.Email 
                                                         && user.Gender == details.Gender);
        }
        
        [Theory]
        [MemberData(nameof(GenerateNewUserDetails), 20, 2)]
        public void UpdatingUserReturnsUpdatedUser(UserDetails initialDetails, UserDetails details)
        {
            var usersStorage = new Dictionary<int, User>
            {
                [0] = new User(0, initialDetails.Username, initialDetails.Email, initialDetails.Password, initialDetails.Gender, TestRoles[initialDetails.RoleId])
            };
            var users = new TestUsersService(TestRoles, usersStorage);
            var auth = new TestAuthService(users);
            var controller = new UsersController(users, auth, auth);
            var response = controller.UpdateUser(0, new UserUpdateDetails(details.Username, details.Email, details.Gender));
            var returnedUser = ((OkNegotiatedContentResult<User>)response).Content;
            Assert.Equal(0, returnedUser.Id);
            Assert.Equal(initialDetails.Password, returnedUser.Password);
            Assert.Equal(details.Username, returnedUser.Username);
            Assert.Equal(details.Email, returnedUser.Email);
            Assert.Equal(details.Gender, returnedUser.Gender);
        }
    }
}