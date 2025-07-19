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

    public async Task<bool> ExistsAsync(TimeSpan startSession, TimeSpan endSession, CancellationToken cancellationToken)
    {
        return await DbSet.AnyAsync(p => 
            p.Status == PlanStatus.Active 
            && p.StartSession < endSession && p.EndSession > startSession,
            cancellationToken);
    }

    public async Task<Plan?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await DbSet.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }
}