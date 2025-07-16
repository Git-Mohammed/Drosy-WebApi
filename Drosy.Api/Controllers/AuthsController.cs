using Drosy.Application.UseCases.Authentication.Interfaces;
using Drosy.Application.UsesCases.Users.DTOs;
using Drosy.Api.Commons.Responses;
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
            var result = await _authService.LoginAsync(user, token);

            if (result.IsFailure)
                return ApiResponseFactory.UnauthorizedResponse("login", result.Error.Message);

            SetRefreshTokenInCookie(result.Value.RefreshToken, result.Value.RefreshTokenExpiration);

            return ApiResponseFactory.SuccessResponse(result.Value, "Login successful");
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshTokenAsync(CancellationToken token)
        {
            string? refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
                return ApiResponseFactory.UnauthorizedResponse("Access Token", "Unauthorized");

            var result = await _authService.RefreshTokenAsync(refreshToken, token);

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
            
            string? refreshToken = Request.Cookies["refreshToken"];
            if(string.IsNullOrEmpty(refreshToken))
                return ApiResponseFactory.UnauthorizedResponse("Access Token", "Unauthorized");
            
            var result = await _authService.LogoutAsync(refreshToken, cancellationToken);

            if (result.IsFailure)
                return ApiResponseFactory.CreateStatusResponse(500, "logout", "An error occurred during logout");

            return ApiResponseFactory.SuccessResponse("Logged out successfully");
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
