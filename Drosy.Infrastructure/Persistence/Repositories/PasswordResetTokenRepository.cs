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

        public async Task<PasswordResetToken?> GetTokenAsync(string token, CancellationToken ct)
        {
            return await (from restToken in DbSet
                          join user in _dbContext.Users on restToken.UserId equals user.Id
                          where restToken.IsUsed == false && restToken.ExpirationDate > DateTime.UtcNow && token == restToken.TokenString
                          select new PasswordResetToken
                          {
                              ExpirationDate = restToken.ExpirationDate,
                              UserId = user.Id,
                              Id = restToken.Id,
                              IsUsed = restToken.IsUsed,
                              TokenString = restToken.TokenString,
                              User = new AppUser
                              {
                                  Id = user.Id,
                                  Email = user.Email,
                                  UserName = user.UserName
                              }

                          }).FirstOrDefaultAsync(ct);
        }
    }
}