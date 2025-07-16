using Drosy.Domain.Entities;

namespace Drosy.Domain.Interfaces.Repository
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(RefreshToken entity, CancellationToken cancellationToken);
        Task UpdateAsync(RefreshToken entity, CancellationToken cancellationToken);
        Task<RefreshToken?> GetByTokenAsync(string tokenString, CancellationToken cancellationToken);
        Task<RefreshToken?> GetByUserIdAsync(int userId, CancellationToken cancellationToken);
        Task<IEnumerable<RefreshToken>> GetAllByUserIdAsync(int userId, CancellationToken cancellationToken);
    }
}
