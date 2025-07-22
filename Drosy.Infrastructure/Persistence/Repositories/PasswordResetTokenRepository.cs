using Drosy.Domain.Entities;
using Drosy.Domain.Interfaces.Repository;
using Drosy.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Drosy.Infrastructure.Persistence.Repositories
{
    public class PasswordResetTokenRepository : IPasswordResetTokenRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly DbSet<PasswordResetToken> DbSet;

        public PasswordResetTokenRepository(ApplicationDbContext context)
        {
            _dbContext = context;
            DbSet = context.PasswordResetTokens;
        }
        public async Task AddAsync(PasswordResetToken entity, CancellationToken cancellationToken)
        {
            if (entity != null)
                await DbSet.AddAsync(entity);
        }

        public async Task UpdateAsync(PasswordResetToken entity, CancellationToken cancellationToken)
        {
            if (entity != null)
                await Task.Run(() => DbSet.Update(entity));
        }
    }
}