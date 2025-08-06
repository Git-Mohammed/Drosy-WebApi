using Drosy.Domain.Entities;
using Drosy.Domain.Interfaces.Common.Repository;

namespace Drosy.Domain.Interfaces.Repository
{
    public interface IAppUserRepository : IRepository<AppUser>
    {
        Task<AppUser?> FindByIdAsync(int id, CancellationToken ct);
        Task<AppUser?> FindByUsernameAsync(string userName);
    }
}
