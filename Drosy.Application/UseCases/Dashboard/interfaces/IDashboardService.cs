using Drosy.Application.UseCases.Dashboard.DTOs;
using Drosy.Domain.Shared.ApplicationResults;
namespace Drosy.Application.UseCases.Dashboard.interfaces
{
    public interface IDashboardService
    {
        Task<Result<DashboardDTO>> GetDashboardAsync(CancellationToken cancellationToken);
    }
}
