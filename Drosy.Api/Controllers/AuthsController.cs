using System.Security.Claims;
using Drosy.Application.UseCases.Authentication.Interfaces;
using Drosy.Application.UsesCases.Users.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Drosy.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthsController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthsController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(UserLoginDTO user)
        {
            

            var result = await _authService.LoginAsync(user, CancellationToken.None);

            if (result.IsFailure) return BadRequest(result.Error);

            SetRefreshTokenInCookie(result.Value.RefreshToken, result.Value.RefreshTokenExpiration);

            return Ok(new { token = result.Value });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshTokenAsync([FromBody] string tokenString)
        {
            var result = await _authService.RefreshTokenAsync(tokenString, CancellationToken.None);
            if (result.IsFailure) return BadRequest(result.Error);
            SetRefreshTokenInCookie(result.Value.RefreshToken, result.Value.RefreshTokenExpiration);
            return Ok(new { token = result.Value });
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout(CancellationToken cancellationToken)
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId) || userId <= 0)
                return Unauthorized();

            var result = await _authService.LogoutAsync(userId, cancellationToken);

            if (result.IsFailure)
                return StatusCode(500, new { message = "An error occurred during logout" });

            return Ok(new { message = "Logged out successfully" });
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
