using Drosy.Application.UseCases.Plans.DTOs;
using Drosy.Application.UseCases.Sessions.DTOs;
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

        public async Task<bool> ExistsAsync(DateTime date, int planId, DateTime startTime, DateTime endTime, CancellationToken ct)
        => await DbSet.AnyAsync(s =>
                s.PlanId == planId &&
                s.CreatedAt.Date == date.Date &&
                startTime < s.EndTime &&
                endTime > s.StartTime,
                ct);
        public async Task<bool> ExistsAsync(DateTime date, DateTime startTime, DateTime endTime, CancellationToken ct)
        {
            return await DbSet.AnyAsync(s =>
                s.CreatedAt.Date == date.Date &&
                startTime < s.EndTime &&
                endTime > s.StartTime,
                ct);
        }
        public async Task<bool> ExistsAsync(int excludeSessionId, DateTime date, DateTime startTime, DateTime endTime, CancellationToken ct)
        {
            return await DbSet
                .Where(s => s.CreatedAt == date &&
                            s.Id != excludeSessionId &&
                            startTime < s.EndTime &&
                            endTime > s.StartTime)
                .AnyAsync(ct);
        }

        public async Task<Session?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await DbSet.Include(x => x.Plan).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Session>> GetAllSessionsAsync(DateTime date, CancellationToken ct)
            => await DbSet
                .Include(s => s.Plan).Include(s => s.Attendences)
                .ToListAsync(ct);

        public async Task<IEnumerable<Session>> GetAllByDateAsync(DateTime date, CancellationToken ct)
       => await DbSet
            .Include(s => s.Plan).Include(s => s.Attendences)
           .Where(s => s.StartTime.Date == date.Date)
           .ToListAsync(ct);

        public async Task<IEnumerable<Session>> GetAllInRangeAsync(DateTime start, DateTime end, CancellationToken ct)
            => await DbSet
            .Include(s => s.Plan).Include(s => s.Attendences)
                .Where(s => s.StartTime >= start && s.EndTime <= end)
                .ToListAsync(ct);

        public async Task<IEnumerable<Session>> GetAllByWeekAsync(int year, int weekNumber, CancellationToken ct)
        {
            var monday = IsoWeekHelper.GetFirstDateOfIsoWeek(year, weekNumber);
            var sunday = monday.AddDays(6);
            return await GetAllInRangeAsync(monday, sunday, ct);
        }

        public async Task<IEnumerable<Session>> GetAllByMonthAsync(int year, int month, CancellationToken ct)
            => await DbSet
            .Include(s => s.Plan).Include(s => s.Attendences)
                .Where(s => s.StartTime.Year == year && s.StartTime.Month == month)
                .ToListAsync(ct);

        public async Task<IEnumerable<Session>> GetAllByStatusAsync(SessionStatus status, CancellationToken ct)
            => await DbSet
            .Include(s => s.Plan).Include(s => s.Attendences)
                .Where(s => s.Status == status)
                .ToListAsync(ct);

        public async Task<IEnumerable<Session>> GetAllByPlanAsync(int planId, CancellationToken ct)
            => await DbSet
            .Include(s => s.Plan).Include(s => s.Attendences)
                .Where(s => s.PlanId == planId)
                .ToListAsync(ct);

        public async Task<IEnumerable<Session>> GetAllByPlanAndDateAsync(int planId, DateTime date, CancellationToken ct)
            => (await GetAllByPlanAsync(planId, ct))
                .Where(s => s.StartTime.Date == date.Date);

        public async Task<IEnumerable<Session>> GetAllByPlanAndRangeAsync(int planId, DateTime start, DateTime end, CancellationToken ct)
            => (await GetAllByPlanAsync(planId, ct))
                .Where(s => s.StartTime >= start && s.EndTime <= end);

        public async Task<IEnumerable<Session>> GetAllByPlanAndWeekAsync(int planId, int year, int weekNumber, CancellationToken ct)
        {
            var monday = IsoWeekHelper.GetFirstDateOfIsoWeek(year, weekNumber);
            var sunday = monday.AddDays(6);
            return await GetAllByPlanAndRangeAsync(planId, monday, sunday, ct);
        }

        public async Task<IEnumerable<Session>> GetAllByPlanAndMonthAsync(int planId, int year, int month, CancellationToken ct)
            => (await GetAllByPlanAsync(planId, ct))
                .Where(s => s.StartTime.Year == year && s.StartTime.Month == month);

        public async Task<IEnumerable<Session>> GetAllByPlanAndStatusAsync(int planId, SessionStatus status, CancellationToken ct)
            => (await GetAllByPlanAsync(planId, ct))
                .Where(s => s.Status == status);


        #endregion

        #region Write
        #endregion

    }
}
