using Drosy.Api.Commons.Responses;
using Drosy.Application.UseCases.Plans.DTOs;
using Drosy.Application.UseCases.Plans.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Drosy.Api.Controllers;

/// <summary>
/// Controller for managing educational plans.
/// </summary>
[ApiController]
[Route("api/plans")]
[Authorize]
public class PlansController(IPlanService planService, ILogger<PlansController> logger) : ControllerBase
{
    private readonly IPlanService _planService = planService;
    private readonly ILogger<PlansController> _logger = logger;

    /// <summary>
    /// Retrieves a plan by its ID.
    /// </summary>
    /// <param name="id">The ID of the plan.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The details of the specified plan.</returns>
    [HttpGet("{id}", Name = "GetPlanById")]
    [ProducesResponseType(typeof(ApiResponse<PlanDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PlanDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var result = await _planService.GetPlanByIdAsync(id, cancellationToken);
        const string operation = nameof(GetByIdAsync);
        if (result.IsSuccess)
            return ApiResponseFactory.SuccessResponse(result.Value);

        _logger.LogError("Error retrieving plan [{id}]: {Error}", id, result.Error);
        return ApiResponseFactory.FromFailure(result, operation, nameof(PlanDto));
    }

    /// <summary>
    /// Retrieves all plans without their full details.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of all existing plans.</returns>
    [HttpGet(Name = "GetAllPlansWithoutDetails")]
    [ProducesResponseType(typeof(ApiResponse<List<PlanDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PlanDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAllPlansWithoutDetails(CancellationToken cancellationToken)
    {
        var result = await _planService.GetAllPlansAsync(cancellationToken);
        const string operation = nameof(GetAllPlansWithoutDetails);
        if (result.IsSuccess)
            return ApiResponseFactory.SuccessResponse(result.Value);

        _logger.LogError("Error retrieving plans: {Error}", result.Error);
        return ApiResponseFactory.FromFailure(result, operation, nameof(PlanDto));
    }

    /// <summary>
    /// Creates a new plan.
    /// </summary>
    /// <param name="newPlan">The plan to be created.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created plan and its ID.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<PlanDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<PlanDto>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAsync(CreatePlanDto newPlan, CancellationToken cancellationToken)
    {
        var result = await _planService.CreatePlanAsync(newPlan, cancellationToken);
        const string operation = nameof(CreateAsync);

        if (result.IsSuccess)
            return ApiResponseFactory.CreatedResponse("GetPlanById", new { id = result.Value.Id }, result.Value);

        _logger.LogError("Error creating new plan [{newPlan}]: {Error}", newPlan, result.Error);
        return ApiResponseFactory.FromFailure(result, operation, nameof(PlanDto));
    }

    /// <summary>
    /// Deletes a plan by ID.
    /// </summary>
    /// <param name="id">The ID of the plan to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Status of the delete operation.</returns>
    [HttpDelete("{id}", Name = "DeletePlanById")]
    [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteByIdAsync(int id, CancellationToken cancellationToken)
    {
        var result = await _planService.DeleteAsync(id, cancellationToken);
        const string operation = nameof(DeleteByIdAsync);

        if (result.IsSuccess)
            return ApiResponseFactory.SuccessResponse();

        _logger.LogError("Error deleting plan [{id}]: {Error}", id, result.Error);
        return ApiResponseFactory.FromFailure(result, operation, nameof(PlanDto));
    }

    /// <summary>
    /// Updates the status of an existing plan.
    /// </summary>
    /// <param name="id">The ID of the plan to update.</param>
    /// <param name="planStatus">The new status to assign.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Status of the update operation.</returns>
    [HttpPatch("{id}", Name = "UpdatePlanStatus")]
    [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdatePlanStatusAsync(int id, UpdatePlanStatusDto planStatus, CancellationToken cancellationToken)
    {
        var result = await _planService.UpdateStatusAsync(id, planStatus, cancellationToken);
        const string operation = nameof(UpdatePlanStatusAsync);

        if (result.IsSuccess)
            return ApiResponseFactory.SuccessResponse();

        _logger.LogError("Error updating plan [{id}]: {Error}", id, result.Error);
        return ApiResponseFactory.FromFailure(result, operation, nameof(PlanDto));
    }
}
