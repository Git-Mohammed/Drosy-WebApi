using Drosy.Domain.Entities;
using Drosy.Domain.Interfaces.Repository;
using Drosy.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Drosy.Infrastructure.Persistence.Repositories
{
    public class RegionRepository : BaseRepository<Region>, IRegionRepository
    {
        public RegionRepository(ApplicationDbContext dbContext) : base(dbContext) { }
        public async Task<Region?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await DbSet.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<bool> IsRegionUnusedAsync(int regionId, CancellationToken ct)
        {
            return !await DbContext.Students.AnyAsync(s => s.City.RegionId == regionId, ct);
        }

    }
}