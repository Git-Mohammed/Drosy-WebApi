using Drosy.Api.Commons.Responses;
using Drosy.Application.UseCases.Plans.DTOs;
using Drosy.Application.UseCases.Plans.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Drosy.Api.Controllers;

/// <summary>
/// Controller for managing plan-related operations.
/// </summary>
[ApiController]
[Route("api/plans")]
[Authorize]
public class PlansController(IPlanService planService, ILogger<PlansController> logger) : ControllerBase
{
    private readonly IPlanService _planService = planService;
    private readonly ILogger<PlansController> _logger = logger;

    /// <summary>
    /// Retrieves a plan by its unique identifier.
    /// </summary>
    /// <param name="id">The ID of the plan.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The plan details if found; otherwise, an error response.</returns>
    [HttpGet]
    [Route("{id}", Name = "GetPlanById")]
    public async Task<IActionResult> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var result = await _planService.GetPlanByIdAsync(id, cancellationToken);
        const string operation = nameof(GetByIdAsync);
        if (result.IsSuccess) return ApiResponseFactory.SuccessResponse(result.Value);
        _logger.LogError("Error retrieving plan [{id}]: {Error}", id, result.Error);
        return ApiResponseFactory.FromFailure(result, operation, nameof(PlanDto));
    }

    /// <summary>
    /// Creates a new plan.
    /// </summary>
    /// <param name="newPlan">The plan details to be created.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The created plan with its identifier.</returns>
    [HttpPost]
    public async Task<IActionResult> CreateAsync(CreatePlanDto newPlan, CancellationToken cancellationToken)
    {
        var result = await _planService.CreatePlanAsync(newPlan, cancellationToken);
        const string operation = nameof(CreateAsync);
        if (result.IsSuccess)
            return ApiResponseFactory.CreatedResponse("GetPlanById", new { id = result.Value.Id }, result.Value);
        _logger.LogError("Error creating new plan [{newPlan}]: {Error}", newPlan, result.Error);
        return ApiResponseFactory.FromFailure(result, operation, nameof(PlanDto));

    }
}
