using Drosy.Domain.Entities;
using Drosy.Domain.Interfaces.Common.Repository;

namespace Drosy.Domain.Interfaces.Repository;

public interface IPlanRepository : IRepository<Plan>
{
    Task<bool> ExistsAsync(int Id,CancellationToken cancellationToken);
    Task<bool> ExistsAsync(TimeSpan startSession, TimeSpan endSession , CancellationToken cancellationToken);
    Task<Plan?> GetByIdAsync(int id, CancellationToken cancellationToken);
}