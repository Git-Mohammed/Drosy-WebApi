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

        var overlappingExists = await DbContext.Set<PlanDay>()
            .Where(pd => pd.Plan.Status == PlanStatus.Active)
            .Where(pd => dayValues.Contains(pd.Day))
            .Where(pd =>
                days.Any(input =>
                    input.Day == pd.Day &&
                    input.StartSession < pd.EndSession &&
                    input.EndSession > pd.StartSession))
            .AnyAsync(cancellationToken);

        return overlappingExists;
    }


    public async Task<Plan?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await DbSet.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }


    public async Task<IEnumerable<Plan>> GetByDateAsync(DateTime date, CancellationToken cancellationToken)
        => await DbSet.AsNoTracking()
            .Where(p => p.StartDate.Date <= date.Date && p.EndDate.Date >= date.Date)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<Plan>> GetInRangeAsync(DateTime start, DateTime end, CancellationToken cancellationToken)
        => await DbSet.AsNoTracking()
            .Where(p => p.StartDate.Date >= start.Date && p.EndDate.Date <= end.Date)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<Plan>> GetByStatusAsync(PlanStatus status, CancellationToken cancellationToken)
        => await DbSet.AsNoTracking()
            .Where(p => p.Status == status)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<Plan>> GetByTypeAsync(PlanTypes type, CancellationToken cancellationToken)
        => await DbSet.AsNoTracking()
            .Where(p => p.Type == type)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<Plan>> GetByWeekAsync(int year, int week, CancellationToken cancellationToken)
    {
        var (monday, sunday) = IsoWeekHelper.GetWeekRange(year, week);
        return await DbSet.AsNoTracking()
            .Where(p => p.StartDate.Date >= monday && p.EndDate.Date <= sunday)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Plan>> GetByMonthAsync(int year, int month, CancellationToken cancellationToken)
        => await DbSet.AsNoTracking()
            .Where(p => p.StartDate.Year == year && p.StartDate.Month == month)
            .ToListAsync(cancellationToken);

    #endregion
    #region Write
    #endregion

 
}