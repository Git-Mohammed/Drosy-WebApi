using Drosy.Domain.Entities;
using Drosy.Domain.Interfaces.Repository;
using Drosy.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Drosy.Infrastructure.Persistence.Repositories
{
    public class SystemSettingRepository : BaseRepository<SystemSetting>, ISystemSettingRepository
    {
        public SystemSettingRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public async Task<SystemSetting?> GetAsync(CancellationToken ct)
        {
            return await DbSet.FirstOrDefaultAsync(ct);
        }
    }
}
