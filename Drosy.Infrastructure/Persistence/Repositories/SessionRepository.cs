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

     

//        public async Task<List<ScheduleEntrySessionDto>> GetScheduleSessionsAsync(DateTime from, DateTime to, int? planId = null)
//        {



//            var plansQuery = DbContext.Plans
//                .Include(p => p.PlanDays)
//                .Include(p => p.Students)
//                    .ThenInclude(ps => ps.Student)
//                .Where(p => p.Status != PlanStatus.Inactive)
//                .AsQueryable();

//            if (planId.HasValue)
//                plansQuery = plansQuery.Where(p => p.Id == planId.Value);

//            var planList = await plansQuery.ToListAsync();

//            var sessions = await DbSet
//                .Where(s => s.Date >= from && s.Date <= to)
//                .ToListAsync();

//            var result = new List<ScheduleEntrySessionDto>();

//            foreach (var plan in planList)
//            {
//                var days = plan.PlanDays;
//                var students = plan.Students;

//                // Loop over each date in the range
//                foreach (var date in EachDay(from, to))
//                {
//                    var dayName = date.DayOfWeek.ToString();
//                    /*
//                     public class PlanDay
//{
//    public int Id { get; set; }
//    public int PlanId { get; set; }
//    public Days Day { get; set; }
//    public TimeSpan StartSession { get; set; }
//    public TimeSpan EndSession { get; set; }

//    #region Navigation Properties
//    public Plan Plan { get; set; } = new();
//    #endregion
//}
//                    public enum Days : byte
//{
//    None      = 0,
//    Sunday    = 1,    // 0000001
//    Monday    = 2,    // 0000010
//    Tuesday   = 4,    // 0000100
//    Wednesday = 8,    // 0001000
//    Thursday  = 16,   // 0010000
//    Friday    = 32,   // 0100000
//    Saturday  = 64    // 1000000
//}
//                     */
//                    var matchingDay = days.FirstOrDefault(d => d.Day == (Days)dayName);
//                    if (matchingDay == null) continue;

//                    var existingSession = sessions.FirstOrDefault(s =>
//                        s.PlanId == plan.Id &&
//                        s.Date.Date == date.Date &&
//                        s.StartTime == matchingDay.StartSession &&
//                        s.EndTime == matchingDay.EndSession);

//                    var dto = new ScheduleEntrySessionDto
//                    {
//                        ExcepectedDate = date,
//                        PlanId = plan.Id,
//                        PlanType = plan.Type,
//                        PlanStatus = plan.Status,
//                        SessionPeriod = existingSession?.SessionPeriod ?? 0,
//                        Period = existingSession?.Period ?? 0,
//                        Days = new List<PlanDayDto>
//                {
//                    new PlanDayDto
//                    {
//                        Day = matchingDay.Day,
//                        StartSession = matchingDay.StartSession,
//                        EndSession = matchingDay.EndSession
//                    }
//                },
//                        Students = students.Select(s => new ScheduleEntryPlanStudentDto
//                        {
//                            StudentId = s.StudentId,
//                            Notes = s.Notes,
//                            Fee = s.Fee,
//                            CreatedAt = s.CreatedAt,
//                            FirstName = s.Student.FirstName,
//                            SecondName = s.Student.SecondName,
//                            ThirdName = s.Student.ThirdName,
//                            LastName = s.Student.LastName,
//                            Address = s.Student.Address,
//                            PhoneNumber = s.Student.PhoneNumber
//                        }).ToList()
//                    };

//                    result.Add(dto);
//                }
//            }

//            return result;
//        }


        // Helper: iterate each date in range
        private IEnumerable<DateTime> EachDay(DateTime from, DateTime to)
        {
            for (var day = from.Date; day <= to.Date; day = day.AddDays(1))
                yield return day;
        }


        #region Read

        public async Task<Session?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await DbSet.Include(x => x.Plan).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Session>> GetAllSessionsAsync(DateTime date, CancellationToken ct)
            => await DbSet
                .Include(s => s.Plan).Include(s => s.Attendences)
                .ToListAsync(ct);

        public async Task<IEnumerable<Session>> GetSessionsByDateAsync(DateTime date, CancellationToken ct)
       => await DbSet
            .Include(s => s.Plan).Include(s => s.Attendences)
           .Where(s => s.StartTime.Date == date.Date)
           .ToListAsync(ct);

        public async Task<IEnumerable<Session>> GetSessionsInRangeAsync(DateTime start, DateTime end, CancellationToken ct)
            => await DbSet
            .Include(s => s.Plan).Include(s => s.Attendences)
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
            .Include(s => s.Plan).Include(s => s.Attendences)
                .Where(s => s.StartTime.Year == year && s.StartTime.Month == month)
                .ToListAsync(ct);

        public async Task<IEnumerable<Session>> GetSessionsByStatusAsync(SessionStatus status, CancellationToken ct)
            => await DbSet
            .Include(s => s.Plan).Include(s => s.Attendences)
                .Where(s => s.Status == status)
                .ToListAsync(ct);

        public async Task<IEnumerable<Session>> GetByPlanAsync(int planId, CancellationToken ct)
            => await DbSet
            .Include(s => s.Plan).Include(s => s.Attendences)
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



        #endregion

        #region Write
        #endregion

    }
}
