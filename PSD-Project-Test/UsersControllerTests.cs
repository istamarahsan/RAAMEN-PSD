using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http.Results;
using Bogus;
using Bogus.DataSets;
using Moq;
using PSD_Project.API.Features.Authentication;
using PSD_Project.API.Features.Users;
using PSD_Project.API.Features.Users.Authorization;
using PSD_Project.App.Common;
using Util.Option;
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
        private readonly UsersController sut;
        private readonly Mock<IUsersService> usersServiceMock = new Mock<IUsersService>();

        public UsersControllerTests()
        {
            sut = new UsersController(usersServiceMock.Object, authenticationServiceMock.Object,
                authorizationServiceMock.Object);
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

        public static IEnumerable<object[]> GetTestRoles()
        {
            return TestRoles.Values.Select(r => new object[] { r });
        }

        public static IEnumerable<object[]> GetTestRoleIds()
        {
            return TestRoles.Values.Select(r => new object[] { r.Id });
        }

        [Fact]
        public void InvalidToken_On_GetUsers_Returns_Unauthorized()
        {
            authenticationServiceMock.EmptySessionWithRoleId(0);
            authorizationServiceMock.AllowAny();
            sut.Request = new HttpRequestMessage();
            sut.Request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "");
            var getAll = sut.GetAllUsers();
            Assert.IsType<UnauthorizedResult>(getAll);
        }

        [Fact]
        public void InvalidToken_On_GetUser_Returns_Unauthorized()
        {
            var gen = UserDetailsGenerator().First();
            var usr = new User(0, gen.Username, gen.Email, gen.Password, gen.Gender, new RoleDetails(0, ""));
            authenticationServiceMock.EmptySessionWithRoleId(0);
            authorizationServiceMock.AllowAny();
            usersServiceMock.Setup(service => service.GetUser(It.IsAny<int>()))
                .Returns(Try.Of<User, Exception>(usr));
            sut.Request = new HttpRequestMessage();
            sut.Request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "");
            var getAll = sut.GetAllUsers();
            Assert.IsType<UnauthorizedResult>(getAll);
        }

        [Fact]
        public void InvalidToken_On_GetUsersWithRole_Returns_Unauthorized()
        {
            authenticationServiceMock.EmptySessionWithRoleId(0);
            authorizationServiceMock.AllowAny();
            usersServiceMock.Setup(service => service.GetUsersWithRole(It.IsAny<int>()))
                .Returns(Try.Of<List<User>, Exception>(new List<User>()));
            sut.Request = new HttpRequestMessage();
            sut.Request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "");
            var getAll = sut.GetUsersWithRole(0);
            Assert.IsType<UnauthorizedResult>(getAll);
        }

        [Fact]
        public void InvalidToken_On_CreateNewUser_Returns_Unauthorized()
        {
            authenticationServiceMock.EmptySessionWithRoleId(0);
            authorizationServiceMock.AllowAny();
            usersServiceMock.Setup(service => service.CreateUser(It.IsAny<UserDetails>()))
                .Returns(Try.Err<User, Exception>(new Exception()));
            sut.Request = new HttpRequestMessage();
            sut.Request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "");
            var getAll = sut.CreateNewUser(UserDetailsGenerator().First());
            Assert.IsType<UnauthorizedResult>(getAll);
        }

        [Fact]
        public void InvalidToken_On_UpdateUser_Returns_Unauthorized()
        {
            authenticationServiceMock.EmptySessionWithRoleId(0);
            authorizationServiceMock.AllowAny();
            usersServiceMock.Setup(service => service.UpdateUser(It.IsAny<int>(), It.IsAny<UserUpdateDetails>()))
                .Returns(Try.Err<User, Exception>(new Exception()));
            sut.Request = new HttpRequestMessage();
            sut.Request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "");
            var getAll = sut.GetAllUsers();
            Assert.IsType<UnauthorizedResult>(getAll);
        }

        [Fact]
        public void GetAllUsers_Returns_AllUsers()
        {
            authenticationServiceMock.EmptySessionWithRoleId(0);
            authorizationServiceMock.AllowAny();
            var users = UserDetailsGenerator().Take(5)
                .Select((gen, i) =>
                    new User(i, gen.Username, gen.Email, gen.Password, gen.Gender, TestRoles[gen.RoleId]))
                .OrderBy(user => user.Id)
                .ToList();

            usersServiceMock.Setup(service => service.GetUsers())
                .Returns(Try.Of<List<User>, Exception>(users));

            var response = sut.WithBearerToken(0, controller => controller.GetAllUsers());
            var responseUsers = ((OkNegotiatedContentResult<List<User>>)response).Content
                .OrderBy(user => user.Id)
                .ToList();

            Assert.IsType<OkNegotiatedContentResult<List<User>>>(response);
            users.Zip(responseUsers, (expected, actual) => (expected, actual))
                .ToList()
                .ForEach(tuple => Assert.Equal(tuple.expected, tuple.actual));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void GetUsersWithRole_Returns_UsersWithRole(int roleId)
        {
            var rand = new Random();
            var roles = Enumerable.Range(1, 3).Select(i => new RoleDetails(i, i.ToString())).ToDictionary(r => r.Id);
            var users = UserDetailsGenerator().Take(30)
                .Select((gen, i) =>
                    new User(i, gen.Username, gen.Email, gen.Password, gen.Gender, roles[rand.Next(1, 3)]))
                .ToList();
            authenticationServiceMock.EmptySessionWithRoleId(0);
            authorizationServiceMock.AllowAny();
            authorizationServiceMock.Setup(service => service.PermissionToRead(It.IsAny<int>()))
                .Returns(Option.Some(Permission.ReadCustomerUserdetails));
            usersServiceMock.Setup(service => service.GetUsersWithRole(roleId))
                .Returns(Try.Of<List<User>, Exception>(users.Where(u => u.Role.Id == roleId).ToList()));

            var response = sut.WithBearerToken(0, controller => controller.GetUsersWithRole(roleId));
            var responseUsers = (response as OkNegotiatedContentResult<List<User>>)?
                .Content?
                .ToList();

            Assert.IsType<OkNegotiatedContentResult<List<User>>>(response);
            Assert.NotNull(responseUsers);
            users.Where(u => u.Role.Id == roleId).OrderBy(u => u.Id).Zip(responseUsers.OrderBy(u => u.Id),
                    (expected, actual) => (expected, actual))
                .ToList()
                .ForEach(tuple => Assert.Equal(tuple.expected, tuple.actual));
        }

        [Theory]
        [MemberData(nameof(GenerateNewUserDetails), 20, 1)]
        public void CreateUser_Stores_User_In_UsersService(UserDetails form)
        {
            authenticationServiceMock.EmptySessionWithRoleId(0);
            authorizationServiceMock.AllowAny();
            var wasCalled = false;
            usersServiceMock.Setup(service => service.CreateUser(form))
                .Callback(() => wasCalled = true)
                .Returns(() => Try.Err<User, Exception>(new Exception()));
            sut.WithBearerToken(0, controller => controller.CreateNewUser(form));
            Assert.True(wasCalled);
        }


        [Theory]
        [MemberData(nameof(GenerateNewUserDetails), 20, 1)]
        public void NoPermission_On_CreateUser_Returns_UnauthorizedResult(UserDetails form)
        {
            authenticationServiceMock.SessionWithDetails(form, 0);
            authorizationServiceMock
                .Setup(service => service.RoleHasPermission(It.IsAny<int>(), It.IsAny<Permission>()))
                .Returns(false);
            usersServiceMock.Setup(service => service.CreateUser(It.IsAny<UserDetails>()))
                .Returns(Try.Err<User, Exception>(new Exception()));
            var response = sut.WithBearerToken(0, controller => controller.CreateNewUser(form));
            Assert.IsType<UnauthorizedResult>(response);
        }

        [Theory]
        [MemberData(nameof(GenerateNewUserDetails), 20, 1)]
        public void UnauthorizedAccessException_On_CreateUser_From_AuthenticationService_MapsTo_UnauthorizedResult(
            UserDetails form)
        {
            authenticationServiceMock.Setup(service => service.GetSession(It.IsAny<int>()))
                .Returns(() => Try.Err<UserSessionDetails, Exception>(new UnauthorizedAccessException()));
            authorizationServiceMock.AllowAny();
            var response = sut.WithBearerToken(0, controller => controller.CreateNewUser(form));
            Assert.IsType<UnauthorizedResult>(response);
        }

        [Theory]
        [MemberData(nameof(GenerateNewUserDetails), 20, 1)]
        public void ArgumentException_On_CreateUser_From_UsersService_MapsTo_BadRequest(UserDetails form)
        {
            authenticationServiceMock.EmptySessionWithRoleId(0);
            authorizationServiceMock.AllowAny();
            usersServiceMock.Setup(service => service.CreateUser(form))
                .Returns(Try.Err<User, Exception>(new ArgumentException()));

            var response = sut.WithBearerToken(0, controller => controller.CreateNewUser(form));

            Assert.IsType<BadRequestResult>(response);
        }

        [Theory]
        [MemberData(nameof(GenerateNewUserDetails), 20, 1)]
        public void GenericException_On_CreateUser_From_UsersService_MapsTo_ExceptionResult(UserDetails form)
        {
            authenticationServiceMock.EmptySessionWithRoleId(0);
            authorizationServiceMock.AllowAny();
            usersServiceMock.Setup(service => service.CreateUser(form))
                .Returns(Try.Err<User, Exception>(new Exception()));
            var response = sut.WithBearerToken(0, controller => controller.CreateNewUser(form));
            Assert.IsType<ExceptionResult>(response);
        }
    }
}