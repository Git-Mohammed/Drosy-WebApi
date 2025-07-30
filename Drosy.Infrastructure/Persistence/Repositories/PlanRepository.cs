using Drosy.Domain.Entities;
using Drosy.Domain.Enums;
using Drosy.Domain.Interfaces.Repository;
using Drosy.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Drosy.Infrastructure.Persistence.Repositories;

public class PlanRepository(ApplicationDbContext dbContext) : BaseRepository<Plan>(dbContext), IPlanRepository
{
    public async Task<bool> ExistsAsync(int id,CancellationToken cancellationToken)
    {
        return await DbSet.AnyAsync(p => p.Id == id && p.Status == PlanStatus.Active, cancellationToken);
    }

    public async Task<bool> ExistsAsync(List<PlanDay> days, CancellationToken cancellationToken)
    {
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


    public async Task<Plan?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await DbSet
            .Include(p=> p.PlanDays)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }
}