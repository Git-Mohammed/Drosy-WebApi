using System.Security.Claims;
using Drosy.Api.Commons.Responses;
using Drosy.Application.UseCases.Authentication.Interfaces;
using Drosy.Application.UseCases.Email.Interfaces;
using Drosy.Application.UsesCases.Users.DTOs;
using Drosy.Domain.Shared.ApplicationResults;
using Drosy.Domain.Shared.ErrorComponents.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Drosy.Api.Controllers
{
    [Route("api/auths")]
    [ApiController]
    [Authorize]
    public class AuthsController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthsController(IAuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(UserLoginDTO user, CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                return ApiResponseFactory.FromFailure(Result.Failure(CommonErrors.OperationCancelled), nameof(LoginAsync));
            }
            var result = await _authService.LoginAsync(user, token);

            if (result.IsFailure)
                return ApiResponseFactory.BadRequestResponse("login", result.Error.Message);

            SetRefreshTokenInCookie(result.Value.RefreshToken, result.Value.RefreshTokenExpiration);

            return ApiResponseFactory.SuccessResponse(result.Value, "Login successful");
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshTokenAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return ApiResponseFactory.FromFailure(Result.Failure(CommonErrors.OperationCancelled), nameof(LoginAsync));
            }
            string? refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
                return ApiResponseFactory.UnauthorizedResponse("Access Token", "Unauthorized");

            var result = await _authService.RefreshTokenAsync(refreshToken, cancellationToken);

            if (result.IsFailure)
            {
                if (result.Error.Code == "NullValue")
                    return ApiResponseFactory.BadRequestResponse("token", result.Error.Message);

                return ApiResponseFactory.UnauthorizedResponse("token", result.Error.Message);
            }

            SetRefreshTokenInCookie(result.Value.RefreshToken, result.Value.RefreshTokenExpiration);

            return ApiResponseFactory.SuccessResponse(result.Value, "Token refreshed successfully");
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout(CancellationToken cancellationToken)
        {
            // if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId) || userId <= 0)
            //     return ResponseHandler.UnauthorizedResponse("user", "Invalid or missing user ID");

            if (cancellationToken.IsCancellationRequested)
            {
                return ApiResponseFactory.FromFailure(Result.Failure(CommonErrors.OperationCancelled), nameof(LoginAsync));
            }

            string? refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
                return ApiResponseFactory.UnauthorizedResponse("Access Token", "Unauthorized");

            var result = await _authService.LogoutAsync(refreshToken, cancellationToken);

            if (result.IsFailure)
                return ApiResponseFactory.CreateStatusResponse(500, "logout", "An error occurred during logout");

            return ApiResponseFactory.SuccessResponse("Logged out successfully");
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePasswordAsync(ChangePasswordDTO dto ,CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
            {
                return ApiResponseFactory.FromFailure(Result.Failure(CommonErrors.OperationCancelled), nameof(LoginAsync));
            }

            var userId = 0;
            int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out userId);
            var result = await _authService.ChangePasswordAsync(userId, dto, ct);

            if (result.IsFailure)
            {
                return ApiResponseFactory.BadRequestResponse(nameof(ChangePasswordAsync), result.Error.Message);
            }
            return ApiResponseFactory.SuccessResponse("Password Cahnge Succefully");
        }

        private void SetRefreshTokenInCookie(string refreshToken, DateTime expires)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = expires.ToLocalTime(),
                Secure = true,
                IsEssential = true,
                SameSite = SameSiteMode.None
            };

            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }
    }
}
