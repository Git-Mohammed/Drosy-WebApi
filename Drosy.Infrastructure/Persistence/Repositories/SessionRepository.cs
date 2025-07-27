using Drosy.Domain.Entities;
using Drosy.Domain.Enums;
using Drosy.Domain.Interfaces.Repository;
using Drosy.Infrastructure.Persistence.DbContexts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Drosy.Infrastructure.Persistence.Repositories
{
    public class SessionRepository : BaseRepository<Session>, ISessionRepository
    {

        public SessionRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext) { }
        public async Task<Session?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
           return await DbSet.Include(x => x.Plan).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        #region Read
        public async Task<IEnumerable<Session>> GetSessionsByDateAsync(int planId, DateTime date, CancellationToken cancellationToken)
         => await DbSet
         .AsNoTracking().Include(x => x.Plan)
                .Where(s => s.PlanId == planId
                            && s.CreatedAt.Date == date.Date)
                .OrderBy(s => s.StartTime)
                .ToListAsync(cancellationToken);

        public async Task<IEnumerable<Session>> GetSessionsByDateAsync( DateTime date, CancellationToken cancellationToken)
         => await DbSet
         .AsNoTracking().Include(x => x.Plan)
                .Where(s => s.CreatedAt.Date == date.Date)
                .OrderBy(s => s.StartTime)
                .ToListAsync(cancellationToken);
     

        public async Task<IEnumerable<Session>> GetSessionsInRangeAsync(
         DateTime start,
         DateTime end,
         CancellationToken cancellationToken)
         => await DbSet
             .AsNoTracking().Include(x => x.Plan)
             .Where(s =>  s.CreatedAt.Date >= start.Date
                         && s.CreatedAt.Date <= end.Date)
             .OrderBy(s => s.CreatedAt)
             .ThenBy(s => s.StartTime)
             .ToListAsync(cancellationToken);


        public async Task<IEnumerable<Session>> GetSessionsByStatusAsync(
            SessionStatus status,
            CancellationToken cancellationToken)
            => await DbSet.Include(x => x.Plan)
                .AsNoTracking()
                .Where(s =>  s.Status == status)
                .OrderBy(s => s.CreatedAt)
                .ThenBy(s => s.StartTime)
                .ToListAsync(cancellationToken);

        public async Task<bool> ExistsAsync(DateTime date, int planId, DateTime startTime, DateTime endTime, CancellationToken cancellationToken)
        => await DbSet.AnyAsync(s =>
                s.PlanId == planId &&
                s.CreatedAt.Date == date.Date &&
                startTime < s.EndTime &&
                endTime > s.StartTime,
                cancellationToken);
       

        public async Task<bool> ExistsAsync(DateTime date, DateTime startTime, DateTime endTime, CancellationToken cancellationToken)
        {
            return await DbSet.AnyAsync(s =>
                s.CreatedAt.Date == date.Date &&
                startTime < s.EndTime &&
                endTime > s.StartTime,
                cancellationToken);
        }

        public async Task<bool> ExistsAsync(int excludeSessionId, DateTime date, DateTime startTime, DateTime endTime, CancellationToken cancellationToken)
        {
            return await DbSet
                .Where(s => s.CreatedAt == date &&
                            s.Id != excludeSessionId &&
                            startTime < s.EndTime &&
                            endTime > s.StartTime)
                .AnyAsync(cancellationToken);
        }
        #endregion

        #region Write
        #endregion

    }
}
