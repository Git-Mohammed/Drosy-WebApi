using Drosy.Api.Commons.Responses;
using Drosy.Application.UseCases.Plans.DTOs;
using Drosy.Application.UseCases.Plans.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Drosy.Api.Controllers;

[ApiController]
[Route("api/plans")]
public class PlansController(IPlanService planService, ILogger<PlansController> logger) : ControllerBase
{
    private readonly IPlanService _planService = planService;
    private readonly ILogger<PlansController> _logger = logger;

    [HttpGet]
    [Route("{id}",  Name = "GetPlanById")]
    public async Task<IActionResult> GetPlanByIdAsync(int id, CancellationToken cancellationToken)
    {
        var result = await _planService.GetPlanByIdAsync(id,cancellationToken);
        if (!result.IsSuccess)
        {
            _logger.LogError($"Error creating new plan: {result.Error}");
            return ResponseHandler.HandleFailure(result,nameof(CreateNewPlanAsync),nameof(PlanDto));
        }
        return ResponseHandler.SuccessResponse(result.Value);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateNewPlanAsync(CreatePlanDTo newPlan, CancellationToken cancellationToken)
    {
        var result = await _planService.CreatePlanAsync(newPlan, cancellationToken);
        if (!result.IsSuccess)
        {
            _logger.LogError($"Error creating new plan: {result.Error}");
            return ResponseHandler.HandleFailure(result,nameof(CreateNewPlanAsync),nameof(PlanDto));
        }

        return ResponseHandler.CreatedResponse("GetPlanById",new {id = result.Value.Id},result.Value);
    }
}