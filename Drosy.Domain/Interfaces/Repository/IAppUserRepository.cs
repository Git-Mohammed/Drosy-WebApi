using Drosy.Domain.Entities;

namespace Drosy.Domain.Interfaces.Repository
{
    public interface IAppUserRepository : IRepository<AppUser>
    {
        Task<AppUser> FindByIdAsync(int id);
        Task<AppUser?> FindByUsernameAsync(string userName);
    }
}
