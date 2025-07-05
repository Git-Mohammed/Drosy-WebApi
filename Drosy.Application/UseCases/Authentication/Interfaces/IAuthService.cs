using Drosy.Application.UsesCases.Authentication.DTOs;
using Drosy.Application.UsesCases.Users.DTOs;
using Drosy.Domain.Shared.ResultPattern;

namespace Drosy.Application.UsesCases.Authentication.Interfaces
{
    public interface IAuthService
    {
        Task<Result<AuthModel>> LoginAsync(UserLoginDTO user);
        Task<Result<AuthModel>> RefreshTokenAsync(string tokenString);
    }
}
