using Drosy.Application.UseCases.Dashboard.DTOs;
using Drosy.Application.UseCases.Dashboard.interfaces;
using Drosy.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Drosy.Infrastructure.Persistence.Repositories
{
    public class DashboardRepositpory : IDashboardRepositpory
    {
        private readonly DbSet<DashboardStatsViewDTO> _dbSet;
        public DashboardRepositpory(ApplicationDbContext dbContext)
        {
            _dbSet = dbContext.DashboardStats;
        }

        public async Task<DashboardStatsViewDTO?> GetDashboardStatsViewAsync(CancellationToken cancellationToken)
        {
            return await _dbSet.FirstOrDefaultAsync(cancellationToken);
        }
    }
}