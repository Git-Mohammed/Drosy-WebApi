using Drosy.Domain.Entities;
using Drosy.Domain.Interfaces.Repository;
using Drosy.Infrastructure.Persistence.DbContexts;
using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using Microsoft.EntityFrameworkCore;

namespace Drosy.Infrastructure.Persistence.Repositories
{
    /*
     TODO:
        - How to get the user while it's not in the database
     */
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly DbSet<RefreshToken> DbSet;

        public RefreshTokenRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            DbSet = _dbContext.RefreshTokens;
        }

        public async Task AddAsync(RefreshToken entity, CancellationToken cancellationToken)
        {
            await DbSet.AddAsync(entity, cancellationToken);
        }

        public async Task<RefreshToken?> GetByTokenAsync(string tokenString, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(tokenString))
                return null;

            return await DbSet.Include(x => x.User).FirstOrDefaultAsync(x => x.Token == tokenString && x.RevokedOn == null && DateTime.UtcNow <= x.ExpiresOn, cancellationToken);
        }

        public async Task<RefreshToken?> GetByUserIdAsync(int userId, CancellationToken cancellationToken)
        {
            if (userId <= 0)
                return null;

            return await DbSet.FirstOrDefaultAsync(x => x.UserId == userId && x.RevokedOn == null && DateTime.UtcNow <= x.ExpiresOn, cancellationToken);
        }

        public async Task<IEnumerable<RefreshToken>> GetAllByUserIdAsync(int userId, CancellationToken cancellationToken)
        {
            return await DbSet.Where(x => x.UserId == userId && x.RevokedOn == null && DateTime.UtcNow <= x.ExpiresOn).ToListAsync(cancellationToken);
        }

        public Task UpdateAsync(RefreshToken entity, CancellationToken cancellationToken)
        {
            return Task.Run(() => DbSet.Update(entity), cancellationToken);
        }
    }
}