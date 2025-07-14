using Drosy.Domain.Entities;
using Drosy.Domain.Interfaces.Repository;
using Drosy.Infrastructure.Persistence.DbContexts;

namespace Drosy.Infrastructure.Persistence.Repositories;

public class PlanRepository : IPlanRepository
{
    public Task AddAsync(Plan entity, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task AddRangeAsync(IEnumerable<Plan> entities, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Plan entity, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task UpdateRangeAsync(IEnumerable<Plan> entities, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Plan entity, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task DeleteRangeAsync(IEnumerable<Plan> entities, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Plan>> GetAllAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}