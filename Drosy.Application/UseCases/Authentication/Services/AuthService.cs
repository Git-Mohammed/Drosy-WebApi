using System.Security.Claims;
using Drosy.Application.Interfaces.Common;
using Drosy.Application.UseCases.Authentication.Interfaces;
using Drosy.Application.UsesCases.Authentication.DTOs;
using Drosy.Application.UsesCases.Users.DTOs;
using Drosy.Domain.Shared.ApplicationResults;
using Drosy.Domain.Shared.ErrorComponents.Common;

namespace Drosy.Application.UseCases.Authentication.Services
{
    public class AuthService : IAuthService
    {
        private readonly IIdentityService _identityService;
        private readonly IJwtService _jwtService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IJwtService jwtService, IIdentityService identity, ILogger<AuthService> logger)
        {
            _jwtService = jwtService;
            _identityService = identity;
            _logger = logger;
        }

        public async Task<Result<AuthModel>> LoginAsync(UserLoginDTO user, CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                if (user is null) return Result.Failure<AuthModel>(CommonErrors.NullValue);

                var result = await _identityService.PasswordSignInAsync(user.UserName, user.Password, true, true);
                if (result.IsFailure)
                    return Result.Failure<AuthModel>(result.Error);

                var tokenResult = await _jwtService.CreateTokenAsync(result.Value, token);

                if (tokenResult.IsFailure)
                    return Result.Failure<AuthModel>(tokenResult.Error);

                return Result.Success(tokenResult.Value);
            }
            catch (OperationCanceledException) 
            {
                _logger.LogWarning(CommonErrors.OperationCancelled.Message, "Operation Canceld While logging the user with userName {userName}", user.UserName);
                return Result.Failure<AuthModel>(CommonErrors.OperationCancelled);
            }
        }

        public bool IsAuthorized(ClaimsPrincipal user, string requiredRole)
        {
            return user.IsInRole(requiredRole);
        }

        public async Task<Result> LogoutAsync(string refreshToken, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                return await _jwtService.RevokeRefreshTokensAsync(refreshToken,cancellationToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning(CommonErrors.OperationCancelled.Message, "Operation Canceld While logout");
                return Result.Failure<AuthModel>(CommonErrors.OperationCancelled);
            }

        }

        public async Task<Result<AuthModel>> RefreshTokenAsync(string tokenString, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(tokenString)) return Result.Failure<AuthModel>(CommonErrors.NullValue);

            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                return await _jwtService.RefreshTokenAsync(tokenString, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning(CommonErrors.OperationCancelled.Message, "Operation Canceld While Resfreshing the token {tokenString}", tokenString);
                return Result.Failure<AuthModel>(CommonErrors.OperationCancelled);
            }
        }

        public async Task<Result> ChangePasswordAsync(int userId, ChangePasswordDTO dto, CancellationToken ct)
        {
            try
            {
                ct.ThrowIfCancellationRequested();


                var changePasswordResult = await _identityService.ChangePasswordAsync(userId, dto.OldPassword, dto.NewPassword);

                return changePasswordResult.IsSuccess ?
                    Result.Success()
                  : Result.Failure(changePasswordResult.Error);
            }
            catch (OperationCanceledException) 
            {
                _logger.LogWarning(CommonErrors.OperationCancelled.Message, "Operation Canceld While chagning user password {userId}", userId);
                return Result.Failure(CommonErrors.OperationCancelled);
            }
        }

        public async Task<Result> ForgetPasswordAsync(string email, string link, CancellationToken ct)
        {
            try
            {
                ct.ThrowIfCancellationRequested();
                var result = await _identityService.ForgetPasswordAsync(email, link, ct);
                if (result.IsFailure)
                    return Result.Failure(result.Error);

                return result;
            }
            catch(OperationCanceledException)
            {
                _logger.LogWarning(CommonErrors.OperationCancelled.Message, "Operation Canceld While Forget Password Work For user email {email}", email);
                return Result.Failure(CommonErrors.OperationCancelled);
            }
        }

        public async Task<Result> ResetPasswordAsync(RestPasswordDTO dto, CancellationToken ct)
        {
            try
            {
                ct.ThrowIfCancellationRequested();
                var result = await _identityService.RestPasswordAsync(dto, ct);
                if (result.IsFailure)
                    return Result.Failure(result.Error);

                return result;
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning(CommonErrors.OperationCancelled.Message, "Operation Canceld While Rest Password Work");
                return Result.Failure(CommonErrors.OperationCancelled);
            }
        }
    }
}
