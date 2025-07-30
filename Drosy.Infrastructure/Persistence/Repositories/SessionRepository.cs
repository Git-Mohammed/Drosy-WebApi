using Drosy.Domain.Entities;
using Drosy.Domain.Enums;
using Drosy.Domain.Interfaces.Repository;
using Drosy.Domain.Shared.System.CalandeHelper;
using Drosy.Infrastructure.Persistence.DbContexts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Drosy.Infrastructure.Persistence.Repositories
{
    public class SessionRepository : BaseRepository<Session>, ISessionRepository
    {

        public SessionRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext) { }

        #region Read

        public async Task<Session?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await DbSet.Include(x => x.Plan).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }


        public async Task<IEnumerable<Session>> GetSessionsByDateAsync(DateTime date, CancellationToken ct)
       => await DbSet
           .Where(s => s.StartTime.Date == date.Date)
           .ToListAsync(ct);

        public async Task<IEnumerable<Session>> GetSessionsInRangeAsync(DateTime start, DateTime end, CancellationToken ct)
            => await DbSet
                .Where(s => s.StartTime >= start && s.EndTime <= end)
                .ToListAsync(ct);

        public async Task<IEnumerable<Session>> GetSessionsByWeekAsync(int year, int weekNumber, CancellationToken ct)
        {
            var monday = IsoWeekHelper.GetFirstDateOfIsoWeek(year, weekNumber);
            var sunday = monday.AddDays(6);
            return await GetSessionsInRangeAsync(monday, sunday, ct);
        }

        public async Task<IEnumerable<Session>> GetSessionsByMonthAsync(int year, int month, CancellationToken ct)
            => await DbSet
                .Where(s => s.StartTime.Year == year && s.StartTime.Month == month)
                .ToListAsync(ct);

        public async Task<IEnumerable<Session>> GetSessionsByStatusAsync(SessionStatus status, CancellationToken ct)
            => await DbSet
                .Where(s => s.Status == status)
                .ToListAsync(ct);

        public async Task<IEnumerable<Session>> GetByPlanAsync(int planId, CancellationToken ct)
            => await DbSet
                .Where(s => s.PlanId == planId)
                .ToListAsync(ct);

        public async Task<IEnumerable<Session>> GetByPlanAndDateAsync(int planId, DateTime date, CancellationToken ct)
            => (await GetByPlanAsync(planId, ct))
                .Where(s => s.StartTime.Date == date.Date);

        public async Task<IEnumerable<Session>> GetByPlanAndRangeAsync(int planId, DateTime start, DateTime end, CancellationToken ct)
            => (await GetByPlanAsync(planId, ct))
                .Where(s => s.StartTime >= start && s.EndTime <= end);

        public async Task<IEnumerable<Session>> GetByPlanAndWeekAsync(int planId, int year, int weekNumber, CancellationToken ct)
        {
            var monday = IsoWeekHelper.GetFirstDateOfIsoWeek(year, weekNumber);
            var sunday = monday.AddDays(6);
            return await GetByPlanAndRangeAsync(planId, monday, sunday, ct);
        }

        public async Task<IEnumerable<Session>> GetByPlanAndMonthAsync(int planId, int year, int month, CancellationToken ct)
            => (await GetByPlanAsync(planId, ct))
                .Where(s => s.StartTime.Year == year && s.StartTime.Month == month);

        public async Task<IEnumerable<Session>> GetByPlanAndStatusAsync(int planId, SessionStatus status, CancellationToken ct)
            => (await GetByPlanAsync(planId, ct))
                .Where(s => s.Status == status);

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
