using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Results;
using Bogus;
using Bogus.DataSets;
using Moq;
using PSD_Project.API.Features.Authentication;
using PSD_Project.API.Features.Users;
using PSD_Project.API.Features.Users.Authorization;
using PSD_Project.App.Common;
using Util.Try;
using Xunit;

namespace PSD_Project_Test
{
    public class UsersControllerTests
    {
        private static readonly IDictionary<int, RoleDetails> TestRoles = Enumerable.Range(0, 5)
            .Select(i => new RoleDetails(i, $"Role {i}"))
            .ToDictionary(role => role.Id);

        private static readonly Faker Faker = new Faker();
        private readonly Mock<IAuthenticationService> authenticationServiceMock = new Mock<IAuthenticationService>();
        private readonly Mock<IAuthorizationService> authorizationServiceMock = new Mock<IAuthorizationService>();
        private readonly Mock<IUsersService> usersServiceMock = new Mock<IUsersService>();
        private readonly UsersController usersController;

        public UsersControllerTests()
        {
            usersController = new UsersController(usersServiceMock.Object, authenticationServiceMock.Object,
                authorizationServiceMock.Object);
            authenticationServiceMock.Setup(service => service.GetSession(It.IsAny<int>()))
                .Returns(Try.Of<UserSessionDetails, Exception>(new UserSessionDetails(0, "", "", "", "", new RoleDetails(0, ""))));
            authorizationServiceMock.Setup(service => service.RoleHasPermission(It.IsAny<int>(), It.IsAny<Permission>()))
                .Returns(true);
        }

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
            var users = UserDetailsGenerator().Take(5)
                .Select((gen, i) =>
                    new User(i, gen.Username, gen.Email, gen.Password, gen.Gender, TestRoles[gen.RoleId]))
                .OrderBy(user => user.Id)
                .ToList();

            usersServiceMock.Setup(service => service.GetUsers())
                .Returns(Try.Of<List<User>, Exception>(users));

            var response = usersController.WithBearerToken(0, controller => controller.GetAllUsers());
            var responseUsers = ((OkNegotiatedContentResult<List<User>>)response).Content
                .OrderBy(user => user.Id)
                .ToList();
            
            Assert.IsType<OkNegotiatedContentResult<List<User>>>(response);
            users.Zip(responseUsers, (expected, actual) => (expected, actual))
                .ToList()
                .ForEach(tuple => Assert.Equal(tuple.expected, tuple.actual));
        }

        [Theory]
        [MemberData(nameof(GenerateNewUserDetails), 20, 1)]
        public void NewUserAddedIsRetrievable(UserDetails form)
        {
            var usersService = new TestUsersService(TestRoles, new Dictionary<int, User>());
            usersServiceMock.Setup(service => service.GetUsers())
                .Returns(() => usersService.GetUsers());
            usersServiceMock.Setup(service => service.CreateUser(form))
                .Returns(() => usersService.CreateUser(form));
            
            usersController.WithBearerToken(0, controller => controller.CreateNewUser(form));
            var response = usersController.WithBearerToken(0, controller => controller.GetAllUsers());
            var users = (usersController.GetAllUsers() as OkNegotiatedContentResult<List<User>>)?.Content;
            var user = users?.FirstOrDefault();
            
            Assert.IsAssignableFrom<OkNegotiatedContentResult<List<User>>>(response);
            Assert.Equal(1, users?.Count);
            Assert.NotNull(user);
            Assert.Equal(form.Username, user.Username);
            Assert.Equal(form.Password, user.Password);
            Assert.Equal(form.Email, user.Email);
            Assert.Equal(form.Gender, user.Gender);
            Assert.Equal(form.RoleId, user.Role.Id);
        }

        [Theory]
        [MemberData(nameof(GenerateNewUserDetails), 20, 1)]
        public void ArgumentExceptionFromUsersServiceMapsToBadRequest(UserDetails form)
        {
            usersServiceMock.Setup(service => service.CreateUser(form))
                .Returns(Try.Err<User, Exception>(new ArgumentException()));

            var response = usersController.WithBearerToken(0, controller => controller.CreateNewUser(form));

            Assert.IsAssignableFrom<BadRequestResult>(response);
        }
    }
}