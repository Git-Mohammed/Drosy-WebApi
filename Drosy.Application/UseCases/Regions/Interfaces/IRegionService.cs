using Drosy.Application.UseCases.Regions.DTOs;
using Drosy.Domain.Shared.ApplicationResults;
using System.Threading;

namespace Drosy.Application.UseCases.Regions.Interfaces
{
    public interface IRegionService
    {
        Task<Result<DataResult<RegionDTO>>> GetAllAsync(CancellationToken ct);
        Task<Result<RegionDTO>> GetByIdAsync(int id, CancellationToken ct);
        Task<Result<RegionDTO>> CreateAsync(CreateRegionDTO dto, CancellationToken ct);
        Task<Result> UpdateAsync(UpdateRegionDTO dto, int id, CancellationToken ct);
        Task<Result> DeleteAsync(int id, CancellationToken ct);
    }
}
