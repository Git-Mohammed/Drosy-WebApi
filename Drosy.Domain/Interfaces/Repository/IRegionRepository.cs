using Drosy.Domain.Entities;
using Drosy.Domain.Interfaces.Common.Repository;

namespace Drosy.Domain.Interfaces.Repository
{
    public interface IRegionRepository : IRepository<Region>
    {
        Task<Region?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<bool> IsRegionUnusedAsync(int regionId, CancellationToken cancellationToken);


    }
}
