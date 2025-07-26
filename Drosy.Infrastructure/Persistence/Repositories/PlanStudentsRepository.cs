using Drosy.Application.UseCases.Attendences.DTOs;
using Drosy.Domain.Entities;
using Drosy.Domain.Interfaces.Repository;
using Drosy.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Threading;

namespace Drosy.Infrastructure.Persistence.Repositories
{
    public class PlanStudentsRepository : BaseRepository<PlanStudent>, IPlanStudentsRepository
    {
        public PlanStudentsRepository(ApplicationDbContext dbContext) : base(dbContext) {}

        public async Task<bool> ExistsAsync(int planId, int studentId, CancellationToken cancellationToken)
        {
            return await DbSet.AnyAsync(ps => ps.PlanId == planId & ps.StudentId == studentId, cancellationToken);
        }

        public async Task<PlanStudent?> GetById(int planId, int studentId, CancellationToken ct)
        {
            return await DbSet.Include(x => x.Student).Include(x => x.Plan).FirstOrDefaultAsync(ps => ps.PlanId == planId && ps.StudentId == studentId, ct);
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
