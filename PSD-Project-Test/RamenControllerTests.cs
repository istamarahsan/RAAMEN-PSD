using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Results;
using Bogus;
using Moq;
using PSD_Project.API.Features.Authentication;
using PSD_Project.API.Features.Ramen;
using PSD_Project.API.Features.Users.Authorization;
using PSD_Project.App.Common;
using Util.Try;
using Xunit;

namespace PSD_Project_Test
{
    public class RamenControllerTests
    {
        private static readonly Faker Faker = new Faker();

        private static readonly Dictionary<int, Meat> Meats = Enumerable.Range(0, 5)
            .Select(i => new Meat(i, Faker.Commerce.ProductName()))
            .ToDictionary(role => role.Id);

        private readonly Mock<IAuthenticationService> authenticationServiceMock = new Mock<IAuthenticationService>();
        private readonly Mock<IAuthorizationService> authorizationServiceMock = new Mock<IAuthorizationService>();
        private readonly Mock<IRamenService> ramenServiceMock = new Mock<IRamenService>();
        private readonly RamenController sut;

        public RamenControllerTests()
        {
            sut = new RamenController(authenticationServiceMock.Object, authorizationServiceMock.Object,
                ramenServiceMock.Object);
        }

        [Fact]
        public void InvalidToken_On_GetAllRamen_Returns_AllRamen()
        {
            var ramen = GenerateRamenDetails(10).Select((details, i) =>
                new Ramen(i, details.Name, details.Borth, details.Price, Meats[details.MeatId]))
                .ToList();
            authenticationServiceMock.NoSessionsUnauthorizedException();
            authorizationServiceMock.AllowNone();
            ramenServiceMock.Setup(service => service.GetRamen())
                .Returns(Try.Of<List<Ramen>, Exception>(ramen));

            var response = sut.WithBearerToken(0, controller => controller.GetAllRamen())
                .InterpretAs<List<Ramen>>()
                .Unwrap();
            
            Assert.Equal(ramen.OrderBy(r => r.Id), response.OrderBy(r => r.Id));
        }
        
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public void InvalidToken_On_GetRamen_Returns_Ramen(int ramenId)
        {
            var ramen = GenerateRamenDetails(6).Select((details, i) =>
                    new Ramen(i, details.Name, details.Borth, details.Price, Meats[details.MeatId]))
                .OrderBy(r => r.Id)
                .ToList();
            authenticationServiceMock.NoSessionsUnauthorizedException();
            authorizationServiceMock.AllowNone();
            ramenServiceMock.Setup(service => service.GetRamen(It.Is<int>(i => i == ramenId)))
                .Returns(Try.Of<Ramen, Exception>(ramen[ramenId]));

            var response = sut.WithBearerToken(0, controller => controller.GetRamen(ramenId))
                .InterpretAs<Ramen>()
                .Unwrap();
            
            Assert.Equal(ramen[ramenId], response);
        }
        
        [Fact]
        public void UnauthorizedAccessException_From_AuthenticationService_On_CreateRamen_MapsTo_Unauthorized()
        {
            var form = GenerateRamenDetails();
            authenticationServiceMock.NoSessionsUnauthorizedException();
            authorizationServiceMock.AllowAny();
            ramenServiceMock.Setup(service => service.CreateRamen(form))
                .Returns(Try.Of<Ramen, Exception>(new Ramen(0, form.Name, form.Borth, form.Price, Meats[form.MeatId])));

            var response = sut.WithBearerToken(0, controller => controller.CreateRamen(form));

            Assert.IsType<UnauthorizedResult>(response);
        }
        
        [Fact]
        public void NoAccess_From_AuthorizationService_On_CreateRamen_MapsTo_Unauthorized()
        {
            var form = GenerateRamenDetails();
            authenticationServiceMock.EmptySessionWithRoleId(0);
            authorizationServiceMock.AllowNone();
            ramenServiceMock.Setup(service => service.CreateRamen(form))
                .Returns(Try.Of<Ramen, Exception>(new Ramen(0, form.Name, form.Borth, form.Price, Meats[form.MeatId])));

            var response = sut.WithBearerToken(0, controller => controller.CreateRamen(form));

            Assert.IsType<UnauthorizedResult>(response);
        }

        private List<object[]> GenerateRamenDetailsObj(int amountToGenerate)
        {
            return Enumerable.Range(0, amountToGenerate)
                .Select(_ => new[] { GenerateRamenDetails() })
                .Cast<object[]>()
                .ToList();
        }

        private List<RamenDetails> GenerateRamenDetails(int amountToGenerate)
        {
            return Enumerable.Range(0, amountToGenerate)
                .Select(_ => GenerateRamenDetails())
                .ToList();
        }

        private RamenDetails GenerateRamenDetails()
        {
            var ramenName = Faker.Commerce.ProductName();
            var borthName = Faker.Commerce.Department();
            var price = Faker.Commerce.Price();
            var meatId = Faker.PickRandom<int>(Meats.Keys);
            return new RamenDetails(ramenName, borthName, price, meatId);
        }
    }
}