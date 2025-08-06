using Drosy.Domain.Entities;
using Drosy.Domain.Interfaces.Common.Repository;

namespace Drosy.Domain.Interfaces.Repository
{
    public interface ISystemSettingRepository : IRepository<SystemSetting>
    {
        Task<SystemSetting?> GetAsync(CancellationToken ct);
    }
}
