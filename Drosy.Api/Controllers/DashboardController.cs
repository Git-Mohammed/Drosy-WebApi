using Drosy.Api.Commons.Responses;
using Drosy.Application.UseCases.Dashboard.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Drosy.Api.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize]
public class DashboardController(IDashboardService dashboardService, ILogger<DashboardController> logger) : ControllerBase
{
    private readonly IDashboardService _dashboardService = dashboardService;
    private readonly ILogger<DashboardController> _logger = logger;


    [HttpGet(Name = "GetStudentPaymentHistoryAsync")]
    public async Task<IActionResult> GetDashBoardAsync(CancellationToken cancellation)
    {
        if (cancellation.IsCancellationRequested)
            return ApiResponseFactory.BadRequestResponse("GetDashBoardAsync", "Operation Cancled");

        var result = await _dashboardService.GetDashboardAsync(cancellation);

        if (result.IsFailure)
        {
            return ApiResponseFactory.FromFailure(result, nameof(GetDashBoardAsync), "GetDashboardAsync");
        }

        return ApiResponseFactory.SuccessResponse(result.Value, "Dashboard retrieved successfully.");
    }

}