using Drosy.Application.Interfaces.Common;
using Drosy.Application.UseCases.Authentication.Interfaces;
using Drosy.Application.UsesCases.Authentication.DTOs;
using Drosy.Application.UsesCases.Users.DTOs;
using Drosy.Domain.Interfaces.Repository;
using Drosy.Domain.Shared.ResultPattern;
using Drosy.Domain.Shared.ResultPattern.ErrorComponents;
using System.Security.Claims;

namespace Drosy.Application.UseCases.Authentication.Services
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

        public async Task<Result<AuthModel>> LoginAsync(UserLoginDTO user, CancellationToken cancellationToken)
        {
            if (user is null) return Result.Failure<AuthModel>(Error.NullValue);

            var result = await _identityService.PasswordSignInAsync(user.UserName, user.Password, true, true);
            if (result.IsFailure) 
                return Result.Failure<AuthModel>(result.Error);

            var tokenResult = await _jwtService.CreateTokenAsync(result.Value, cancellationToken);

            if (tokenResult.IsFailure)
                return Result.Failure<AuthModel>(tokenResult.Error);

            return Result.Success(tokenResult.Value);
        }

        public bool IsAuthorized(ClaimsPrincipal user, string requiredRole)
        {
            return user.IsInRole(requiredRole);
        }

        public async Task<Result> LogoutAsync(string refreshToken, CancellationToken cancellationToken)
        {
            return await _jwtService.RevokeRefreshTokensAsync(refreshToken,cancellationToken);
        }

        public async Task<Result<AuthModel>> RefreshTokenAsync(string tokenString, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(tokenString)) return Result.Failure<AuthModel>(Error.NullValue);
            return await _jwtService.RefreshTokenAsync(tokenString, cancellationToken);
        }

      
    }
}
