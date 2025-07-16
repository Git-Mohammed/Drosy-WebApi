using Drosy.Application.UsesCases.Authentication.DTOs;
using Drosy.Domain.Entities;
using Drosy.Domain.Shared.ApplicationResults;

namespace Drosy.Application.Interfaces.Common
{
  
    public interface IJwtService 
    {
        Task<Result<AuthModel>> CreateTokenAsync(AppUser user, CancellationToken cancellationToken);
        Task<Result<AuthModel>> RefreshTokenAsync(string tokenString, CancellationToken cancellationToken);
        Task<Result> RevokeRefreshTokensAsync(string refreshToken, CancellationToken cancellationToken);
    }
}