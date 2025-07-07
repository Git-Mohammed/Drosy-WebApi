using Drosy.Domain.Entities;
using Drosy.Domain.Interfaces.Repository;
using Drosy.Infrastructure.Identity.Entities;
using Drosy.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Drosy.Infrastructure.Persistence.Repositories
{
    public class AppUserRepository : BaseRepository<AppUser>, IAppUserRepository
    {
        private readonly DbSet<ApplicationUser> _dbSet;
        public AppUserRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbSet = dbContext.AppUsers;
        }

        public async Task<AppUser?> FindByIdAsync(int id)
        {
            if (id <= 0) return null;
            var user = await _dbSet.FirstOrDefaultAsync(u => u.Id == id);
            if (user is null) return null;
            return new AppUser
            {
                Id = user?.Id ?? 0,
                UserName = user?.UserName ?? string.Empty,
                Email = user?.Email ?? string.Empty,
            };
        }

        public async Task<AppUser?> FindByUsernameAsync(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName)) return null;
            var user = await _dbSet.FirstOrDefaultAsync(u => u.UserName == userName);
            if (user is null) return null;  
            return new AppUser
            {
                Id = user?.Id ?? 0,
                UserName = user?.UserName ?? string.Empty,
                Email = user?.Email ?? string.Empty,
            };
        }
    }
}