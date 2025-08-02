using Drosy.Domain.Entities;
using Drosy.Domain.Enums;
using Drosy.Domain.Interfaces.Repository;
using Drosy.Domain.Shared.System.CalandeHelper;
using Drosy.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Drosy.Infrastructure.Persistence.Repositories;

public class PlanRepository(ApplicationDbContext dbContext) : BaseRepository<Plan>(dbContext), IPlanRepository
{

    #region Read

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken)
    {
        return await DbSet.AnyAsync(p => p.Id == id && p.Status == PlanStatus.Active, cancellationToken);
    }

    public async Task<bool> ExistsAsync(List<PlanDay> days, CancellationToken cancellationToken)
    {

        // return await DbContext.Set<PlanDay>()
        //     .Where(pd => pd.Plan.Status == PlanStatus.Active)
        //     .AnyAsync(pd =>
        //             days.Any(input =>
        //                 pd.Day == input.Day &&
        //                 pd.StartSession < input.EndSession &&
        //                 pd.EndSession > input.StartSession),
        //         cancellationToken);
        // var query = DbContext.Set<PlanDay>().Where(pd => pd.Plan.Status == PlanStatus.Active);
        // var daysQuery = await query.AnyAsync(pd=> days.Where(day=> day.StartSession < pd.EndSession && day.EndSession > pd.StartSession).Count() > 0, cancellationToken);
        // return daysQuery;


        var dayValues = days.Select(d => d.Day).Distinct().ToList();

        var overlappingExists = DbContext.Set<PlanDay>();

        var dayValuse = days.Select(d => d.Day).Distinct().ToList();
        
        // query to get all plan that same days
        var planDays = await DbContext.Set<PlanDay>()

            .Where(pd => pd.Plan.Status == PlanStatus.Active)
            .Where(pd => dayValuse.Contains(pd.Day))
            .ToListAsync(cancellationToken);
        
        // check overlapping 
        foreach (var pd in planDays)
        {
            foreach (var input in days.Where(p => p.Day == pd.Day))
            {
                if (input.StartSession < pd.EndSession && input.EndSession > pd.StartSession)
                    return true;
            }
        }
        return false;

    }

    public async Task<Plan?> GetByIdAsync(int planId, CancellationToken ct)
    => await DbSet
            .Include(p => p.PlanDays)
            .Include(p => p.Sessions)
            .Include(p => p.Students).ThenInclude(ps => ps.Student)
            .FirstOrDefaultAsync(p => p.Id == planId, ct);

    public async Task<IEnumerable<Plan>> GetAllWithDetailsAsync(CancellationToken ct)
     => await DbSet
         .Include(p => p.PlanDays)
         .Include(p => p.Sessions)
         .Include(p => p.Students)
             .ThenInclude(ps => ps.Student)
         .ToListAsync(ct);

    public async Task<IEnumerable<Plan>> GetAllWithDetailsByStatusAsync(PlanStatus status,CancellationToken ct)
     => await DbSet
         .Include(p => p.PlanDays)
         .Include(p => p.Sessions)
         .Include(p => p.Students)
             .ThenInclude(ps => ps.Student)
            .Where(p => p.Status == status)
         .ToListAsync(ct);

    public async Task<IEnumerable<Plan>> GetAllByDateAsync(DateTime date, CancellationToken cancellationToken)
        => await DbSet.AsNoTracking()
             .Include(p => p.PlanDays)
         .Include(p => p.Sessions)
         .Include(p => p.Students)
        .Where(p => p.StartDate.Date <= date.Date && p.EndDate.Date >= date.Date)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<Plan>> GetAllInRangeAsync(DateTime start, DateTime end, CancellationToken cancellationToken)
        => await DbSet.AsNoTracking()
         .Include(p => p.PlanDays)
         .Include(p => p.Sessions)
         .Include(p => p.Students)
            .Where(p => p.StartDate.Date >= start.Date && p.EndDate.Date <= end.Date)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<Plan>> GetAllByStatusAsync(PlanStatus status, CancellationToken cancellationToken)
        => await DbSet
         .Include(p => p.PlanDays)
         .Include(p => p.Sessions)
         .Include(p => p.Students)
             .ThenInclude(ps => ps.Student)
            .Where(p => p.Status == status)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<Plan>> GetAllByTypeAsync(PlanTypes type, CancellationToken cancellationToken)
         => await DbSet
         .Include(p => p.PlanDays)
         .Include(p => p.Sessions)
         .Include(p => p.Students)
             .ThenInclude(ps => ps.Student)
             .Where(p => p.Type == type)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<Plan>> GetAllByWeekAsync(int year, int week, CancellationToken cancellationToken)
    {

        var (monday, sunday) = IsoWeekHelper.GetWeekRange(year, week);
        return await DbSet.AsNoTracking()
             .Include(p => p.PlanDays)
         .Include(p => p.Sessions)
         .Include(p => p.Students)
         .Where(p => p.StartDate.Date >= monday && p.EndDate.Date <= sunday)
            .ToListAsync(cancellationToken);
    }

    public override async Task<IEnumerable<Plan>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await DbSet
            .AsNoTracking()
            .Include(PlanDay => PlanDay.PlanDays)
            .ToListAsync();

    }

    public async Task<IEnumerable<Plan>> GetAllByMonthAsync(int year, int month, CancellationToken cancellationToken)
        => await DbSet.AsNoTracking()
             .Include(p => p.PlanDays)
         .Include(p => p.Sessions)
         .Include(p => p.Students)
        .Where(p => p.StartDate.Year == year && p.StartDate.Month == month)
            .ToListAsync(cancellationToken);

    #endregion
    #region Write
    #endregion

 
}