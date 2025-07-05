using Drosy.Application.UsesCases.Authentication.DTOs;
using Drosy.Application.UsesCases.Authentication.Interfaces;
using Drosy.Application.UsesCases.Users.DTOs;
using Drosy.Application.Interfaces.Common;
using Drosy.Domain.Interfaces.Repository;
using Drosy.Domain.Shared.ResultPattern;
using Drosy.Domain.Shared.ResultPattern.ErrorComponents;
using System.Security.Claims;

namespace Drosy.Application.UsesCases.Authentication.Services
{
    public class AuthService : IAuthService
    {
        private readonly IIdentityService _identityService;
        private readonly IAppUserRepository _userRepository;
        private readonly IJwtService _jwtService;

        public AuthService(IJwtService jwtService, IAppUserRepository userRepository, IIdentityService identity)
        {
            _jwtService = jwtService;
            _userRepository = userRepository;
            _identityService = identity;
        }

        public async Task<Result<AuthModel>> LoginAsync(UserLoginDTO user)
        {
            if (user is null) return Result.Failure<AuthModel>(Error.NullValue);

            var existingUser = await _userRepository.FindByUsernameAsync(user.UserName);

            if (existingUser is null)
                return Result.Failure<AuthModel>(Error.User.InvalidCredentials);

            var result = await _identityService.PasswordSignInAsync(existingUser.UserName, user.Password, true, true);
            if (result.IsFailure) 
                return Result.Failure<AuthModel>(result.Error);

            var tokenResult = await _jwtService.CreateTokenAsync(existingUser);

            if (tokenResult.IsFailure)
                return Result.Failure<AuthModel>(tokenResult.Error);

            return Result.Success(tokenResult.Value);
        }

        public bool IsAuthorized(ClaimsPrincipal user, string requiredRole)
        {
            return user.IsInRole(requiredRole);
        }
        public async Task<Result<AuthModel>> RefreshTokenAsync(string tokenString)
        {
            if (string.IsNullOrEmpty(tokenString)) return Result.Failure<AuthModel>(Error.NullValue);
            return await _jwtService.RefreshTokenAsync(tokenString);
        }
    }
}
