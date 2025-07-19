using Drosy.Application.Interfaces.Common;
using Drosy.Application.UseCases.Authentication.Services;
using Drosy.Application.UsesCases.Authentication.DTOs;
using Drosy.Application.UsesCases.Users.DTOs;
using Drosy.Domain.Entities;
using Drosy.Domain.Interfaces.Repository;
using Drosy.Domain.Shared.ApplicationResults;
using Drosy.Domain.Shared.ErrorComponents;
using Drosy.Domain.Shared.System.Roles;
using Moq;
using System.Security.Claims;
using Drosy.Domain.Shared.ErrorComponents.Common;

namespace Drosy.Tests.Application.Auth
{
    public class AuthorizationServiceTests
    {
        private readonly Mock<IJwtService> _jwtServiceMock;
        private readonly Mock<IAppUserRepository> _userRepoMock;
        private readonly Mock<IIdentityService> _identityServiceMock;
        private readonly Mock<ILogger<AuthService>> _logger;
        private readonly AuthService _authService;

        public AuthorizationServiceTests()
        {
            _jwtServiceMock = new Mock<IJwtService>();
            _userRepoMock = new Mock<IAppUserRepository>();
            _identityServiceMock = new Mock<IIdentityService>();
            _logger = new Mock<ILogger<AuthService>>();
            _authService = new AuthService(
                _jwtServiceMock.Object,
                _userRepoMock.Object,
                _identityServiceMock.Object,
                _logger.Object
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


        [Theory]
        [InlineData("khaled", "correctpass", true)]
        [InlineData("khaled", "wrongpass", false)]
        [InlineData("unknown", "any", false)]
        public async Task LoginAsync_ShouldReturnExpectedResult(string username, string password, bool shouldSucceed)
        {
            // Arrange
            var input = new UserLoginDTO { UserName = username, Password = password };

            // حالة: المستخدم موجود
            if (username == "khaled")
            {
                var user = new AppUser { UserName = "khaled" };

                _userRepoMock.Setup(r => r.FindByUsernameAsync("khaled"))
                             .ReturnsAsync(user, default);

                //if (password == "correctpass")
                //{
                //    _identityServiceMock.Setup(i => i.PasswordSignInAsync("khaled", "correctpass", true, true))
                //                        .ReturnsAsync(Result.Success());

                //    _jwtServiceMock.Setup(j => j.CreateTokenAsync(user, default))
                //                   .ReturnsAsync(Result.Success(new AuthModel()));
                //}
                //else
                //{
                //    _identityServiceMock.Setup(i => i.PasswordSignInAsync("khaled", password, true, true))
                //                        .ReturnsAsync(Result.Failure(Error.User.InvalidCredentials));
                //}
            }
            else
            {
                _userRepoMock.Setup(r => r.FindByUsernameAsync(username))
                             .ReturnsAsync((AppUser)null);
            }

            // Act
            var result = await _authService.LoginAsync(input, default);

            // Assert
            Assert.Equal(shouldSucceed, result.IsSuccess);
        }

        [Theory]
        [InlineData("correctToken", true, false)]
        [InlineData("unCorrectToken", false, true)]
        [InlineData("", false, false)]
        public async Task RefreshTokenAsync_ShouldReturnExpectedResult(string tokenString, bool isValid, bool isFailure)
        {
            // Arrange
            if (string.IsNullOrEmpty(tokenString))
            {
                // No setup needed, will return Error.NullValue
            }
            else if (isValid)
            {
                var authModel = new AuthModel { UserName = "khaled", AccessToken = "newAccessToken" };
                _jwtServiceMock.Setup(j => j.RefreshTokenAsync(tokenString, default))
                    .ReturnsAsync(Result.Success(authModel));
            }
            else if (isFailure)
            {
                _jwtServiceMock.Setup(j => j.RefreshTokenAsync(tokenString, default))
                    .ReturnsAsync(Result.Failure<AuthModel>(AppError.Unauthorized));
            }

            // Act
            var result = await _authService.RefreshTokenAsync(tokenString, default);

            // Assert
            if (string.IsNullOrEmpty(tokenString))
            {
                Assert.False(result.IsSuccess);
                Assert.Equal(CommonErrors.NullValue, result.Error);
            }
            else if (isValid)
            {
                Assert.True(result.IsSuccess);
                Assert.NotNull(result.Value);
                Assert.Equal("khaled", result.Value.UserName);
            }
            else if (isFailure)
            {
                Assert.False(result.IsSuccess);
                Assert.Equal(CommonErrors.Unauthorized, result.Error);
            }
        }

    }
}
