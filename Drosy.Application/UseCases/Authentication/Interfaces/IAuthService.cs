using Drosy.Application.UsesCases.Authentication.DTOs;
using Drosy.Application.UsesCases.Users.DTOs;
using Drosy.Domain.Shared.ResultPattern;
using System.Security.Claims;

namespace Drosy.Application.UseCases.Authentication.Interfaces
{
    public interface IAuthService
    {
        Task<Result<AuthModel>> LoginAsync(UserLoginDTO user, CancellationToken cancellationToken);
        Task<Result<AuthModel>> RefreshTokenAsync(string tokenString, CancellationToken cancellationToken);
        bool IsAuthorized(ClaimsPrincipal user, string requiredRole);
    }
}
