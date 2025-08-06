using Drosy.Application.Interfaces.Common;
using Drosy.Application.UseCases.Dashboard.DTOs;
using Drosy.Application.UseCases.Dashboard.interfaces;
using Drosy.Domain.Shared.ApplicationResults;
using Drosy.Domain.Shared.ErrorComponents.Common;

namespace Drosy.Infrastructure.Dashboard
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepositpory _repo;
        private ILogger<DashboardService> _logger;
        public DashboardService(IDashboardRepositpory dashboardRepositpory, ILogger<DashboardService> logger)
        {
            _repo = dashboardRepositpory;
            _logger = logger;
        }
        public async Task<Result<DashboardDTO>> GetDashboardAsync(CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                var result = await _repo.GetDashboardStatsViewAsync(cancellationToken);

                if (result is null) return Result.Failure<DashboardDTO>(CommonErrors.NullValue);

                var dashboardDTO = new DashboardDTO();
                dashboardDTO.DashboardStats = result;

                return Result.Success(dashboardDTO);
            }
            catch (OperationCanceledException) 
            {
                _logger.LogError("Operation Canceld Exception");
                return Result.Failure<DashboardDTO>(CommonErrors.OperationCancelled);
            }

        }
    }

}
