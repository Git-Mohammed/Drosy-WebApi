using Drosy.Domain.Entities;
using Drosy.Domain.Interfaces.Repository;
using Drosy.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Drosy.Infrastructure.Persistence.Repositories
{
    public class PlanStudentsRepository : BaseRepository<PlanStudent>, IPlanStudentsRepository
    {
        public PlanStudentsRepository(ApplicationDbContext dbContext) : base(dbContext) {}

        public async Task<bool> ExistsAsync(int planId, int studentId, CancellationToken cancellationToken)
        {
            return await DbSet.AnyAsync(ps => ps.PlanId == planId & ps.StudentId == studentId, cancellationToken);
        }

        public async Task<List<int>> GetStudentIdsInPlanAsync(int planId, List<int> studentIds, CancellationToken ct)
        {
            return await DbSet
                .Where(ps => ps.PlanId == planId && studentIds.Contains(ps.StudentId))
                .Select(ps => ps.StudentId)
                .ToListAsync(ct);
        }


    }
}
