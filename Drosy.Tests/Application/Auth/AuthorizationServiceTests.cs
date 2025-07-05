using Xunit;
using System.Security.Claims;
using Drosy.Domain.Shared.System;
using Drosy.Application.UsesCases.Authentication.Services;
using Drosy.Application.Interfaces.Common;
using Drosy.Domain.Interfaces.Repository;
using Moq;

namespace Drosy.Tests.Application.Auth
{
    public class AuthorizationServiceTests
    {
        private readonly Mock<IJwtService> _jwtServiceMock;
        private readonly Mock<IAppUserRepository> _userRepoMock;
        private readonly Mock<IIdentityService> _identityServiceMock;
        private readonly AuthService _authService;

        public AuthorizationServiceTests()
        {
            _jwtServiceMock = new Mock<IJwtService>();
            _userRepoMock = new Mock<IAppUserRepository>();
            _identityServiceMock = new Mock<IIdentityService>();

            _authService = new AuthService(
                _jwtServiceMock.Object,
                _userRepoMock.Object,
                _identityServiceMock.Object
            );
        }

        [Theory]
        [InlineData(AppUserRoles.Admin, AppUserRoles.Admin, true)]
        [InlineData(AppUserRoles.Receptionist, AppUserRoles.Admin, false)]
        [InlineData(AppUserRoles.Student, AppUserRoles.Admin, false)]
        [InlineData(AppUserRoles.Student, AppUserRoles.Student, true)]
        public void IsAuthorized_ShouldMatchExpected(string userRole, string requiredRole, bool expected)
        {
            // Arrange
            var claims = new List<Claim> { new Claim(ClaimTypes.Role, userRole) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var user = new ClaimsPrincipal(identity);

            // Act
            var result = _authService.IsAuthorized(user, requiredRole);

            // Assert
            Assert.Equal(expected, result);
        }
    }

}
