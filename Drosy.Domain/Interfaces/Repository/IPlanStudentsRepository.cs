using Drosy.Domain.Entities;
using Drosy.Domain.Interfaces.Common.Repository;
using System.Linq.Expressions;

namespace Drosy.Domain.Interfaces.Repository
{
    public interface IPlanStudentsRepository : IRepository<PlanStudent>
    {
        Task<bool> ExistsAsync(Expression<Func<PlanStudent, bool>> predicate, CancellationToken cancellationToken);
        Task<List<int>> GetStudentIdsInPlanAsync(int planId, List<int> studentIds, CancellationToken ct);

    }

}
