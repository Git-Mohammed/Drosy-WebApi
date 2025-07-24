using Drosy.Domain.Entities;
using Drosy.Domain.Interfaces.Repository;
using Drosy.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Drosy.Infrastructure.Persistence.Repositories
{
    internal class SessionRepository : BaseRepository<Session>, ISessionRepository
    {

        public SessionRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext) { }
        public async Task<Session?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
           return await DbSet.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Session>> GetSessionsByDateAndPlanAsync(DateTime date, int planId, CancellationToken cancellationToken)
                 => await DbSet
                .Where(s => s.PlanId == planId && s.ExcepectedDate.Date == date.Date)
                .ToListAsync(cancellationToken);
        

        public async Task<bool> SessionExistsAsync(DateTime date, int planId, DateTime startTime, DateTime endTime, CancellationToken cancellationToken)
        {
            return await DbSet.AnyAsync(s =>
                s.PlanId == planId &&
                s.ExcepectedDate.Date == date.Date &&
                startTime < s.EndTime &&
                endTime > s.StartTime,
                cancellationToken);
        }

        public async Task<bool> SessionExistsAsync(DateTime date, DateTime startTime, DateTime endTime, CancellationToken cancellationToken)
        {
            return await DbSet.AnyAsync(s =>
                s.ExcepectedDate.Date == date.Date &&
                startTime < s.EndTime &&
                endTime > s.StartTime,
                cancellationToken);
        }

    }
}
