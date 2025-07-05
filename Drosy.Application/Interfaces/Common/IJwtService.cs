using Drosy.Application.UsesCases.Authentication.DTOs;
using Drosy.Domain.Entities;
using Drosy.Domain.Shared.ResultPattern;

namespace Drosy.Application.Interfaces.Common
{
  
    public interface IJwtService
    {
        Task<Result<AuthModel>> CreateTokenAsync(AppUser user);
        Task<Result<AuthModel>> RefreshTokenAsync(string tokenString);
    }
}