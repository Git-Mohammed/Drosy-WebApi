using Drosy.Domain.Entities;
using Drosy.Domain.Interfaces.Repository;
using Drosy.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Drosy.Infrastructure.Persistence.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly DbSet<RefreshToken> DbSet;

        public RefreshTokenRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            DbSet = _dbContext.RefreshTokens;
        }

        public async Task AddAsync(RefreshToken entity)
        {
            await DbSet.AddAsync(entity);
        }

        public async Task<RefreshToken?> GetByTokenAsync(string tokenString)
        {
            if (string.IsNullOrEmpty(tokenString))
                return null;

            return await DbSet.FirstOrDefaultAsync(x => x.Token == tokenString && x.RevokedOn == null && DateTime.UtcNow >= x.ExpiresOn);
        }

        public async Task<RefreshToken?> GetByUserIdAsync(int userId)
        {
            if (userId <= 0)
                return null;

            return await DbSet.FirstOrDefaultAsync(x => x.UserId == userId && x.RevokedOn == null && DateTime.UtcNow >= x.ExpiresOn);
        }

        public async Task<IEnumerable<RefreshToken>> GetAllByUserIdAsync(int userId)
        {
            return await DbSet.Where(x => x.UserId == userId && x.RevokedOn == null && DateTime.UtcNow >= x.ExpiresOn ).ToListAsync();
        }

        public Task UpdateAsync(RefreshToken entity)
        {
            return Task.FromResult(DbSet.Update(entity));
        }
    }
}