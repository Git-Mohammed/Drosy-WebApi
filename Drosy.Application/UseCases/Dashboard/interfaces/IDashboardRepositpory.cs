using Drosy.Application.UseCases.Dashboard.DTOs;

namespace Drosy.Application.UseCases.Dashboard.interfaces
{
    public interface IDashboardRepositpory
    {
        Task<DashboardStatsViewDTO?> GetDashboardStatsViewAsync(CancellationToken cancellationToken);
    }
}
