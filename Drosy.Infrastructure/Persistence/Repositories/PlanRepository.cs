using Drosy.Domain.Entities;
using Drosy.Domain.Interfaces.Repository;
using Drosy.Infrastructure.Persistence.DbContexts;

namespace Drosy.Infrastructure.Persistence.Repositories;

public class PlanRepository : IPlanRepository
{
    public Task AddAsync(Plan entity)
    {
        throw new NotImplementedException();
    }

    public Task AddRangeAsync(IEnumerable<Plan> entities)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Plan entity)
    {
        throw new NotImplementedException();
    }

    public Task UpdateRangeAsync(IEnumerable<Plan> entities)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Plan entity)
    {
        throw new NotImplementedException();
    }

    public Task DeleteRangeAsync(IEnumerable<Plan> entities)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Plan>> GetAllAsync()
    {
        throw new NotImplementedException();
    }
}