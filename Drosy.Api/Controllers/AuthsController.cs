using Drosy.Application.UsesCases.Authentication.Interfaces;
using Drosy.Application.UsesCases.Users.DTOs;
using Drosy.Domain.Entities;
using Microsoft.AspNetCore.Http;
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
            

            var result = await _authService.LoginAsync(user);

            if (result.IsFailure) return BadRequest(result.Error);

            SetRefreshTokenInCookie(result.Value.RefreshToken, result.Value.RefreshTokenExpiration);

            return Ok(new { token = result.Value });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshTokenAsync([FromBody] string tokenString)
        {
            var result = await _authService.RefreshTokenAsync(tokenString);
            if (result.IsFailure) return BadRequest(result.Error);
            SetRefreshTokenInCookie(result.Value.RefreshToken, result.Value.RefreshTokenExpiration);
            return Ok(new { token = result.Value });
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
