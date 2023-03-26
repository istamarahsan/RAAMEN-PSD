using System;
using Moq;
using PSD_Project.API.Features.Authentication;
using PSD_Project.API.Features.Users;
using PSD_Project.API.Features.Users.Authorization;
using Util.Try;

namespace PSD_Project_Test
{
    public static class MockExtensions
    {
        public static void AllowAny(this Mock<IAuthorizationService> mock)
        {
            mock.Setup(service => service.RoleHasPermission(It.IsAny<int>(), It.IsAny<Permission>()))
                .Returns(true);
        }

        public static void EmptySessionWithRoleId(this Mock<IAuthenticationService> mock, int roleId)
        {
            mock.Setup(service => service.GetSession(It.IsAny<int>()))
                .Returns(Try.Of<UserSessionDetails, Exception>(new UserSessionDetails(0, "", "", "", "",
                    new RoleDetails(roleId, ""))));
        }

        public static void SessionWithDetails(this Mock<IAuthenticationService> mock, UserDetails userDetails,
            int roleId)
        {
            mock.Setup(service => service.GetSession(It.IsAny<int>()))
                .Returns(Try.Of<UserSessionDetails, Exception>(
                    new UserSessionDetails(
                        0,
                        userDetails.Username,
                        userDetails.Email,
                        userDetails.Password,
                        userDetails.Gender,
                        new RoleDetails(
                            roleId,
                            ""))));
        }
    }
}