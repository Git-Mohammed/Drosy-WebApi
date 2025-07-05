using System.Reflection.Metadata;
using Drosy.Domain.Entities;

namespace Drosy.Domain.Interfaces.Repository
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(RefreshToken entity);
        Task UpdateAsync(RefreshToken entity);
        Task<RefreshToken?> GetByTokenAsync(string tokenString);
        Task<RefreshToken?> GetByUserIdAsync(int userId);
    }
}
