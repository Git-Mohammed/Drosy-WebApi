using Drosy.Application.UseCases.SystemSettings.DTOs;
using Drosy.Domain.Shared.ApplicationResults;

namespace Drosy.Application.UseCases.SystemSettings.Interfaces
{
    public interface ISystemSettingService
    {
        Task<Result<SystemSettingDTO>> GetAsync(CancellationToken ct);
        Task<Result<SystemSettingDTO>> UpdateAsync(UpdateSystemSettingDTO dto, CancellationToken ct);
    }
}
