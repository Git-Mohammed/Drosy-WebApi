using Drosy.Application.UseCases.Plans.DTOs;
using Drosy.Domain.Shared.ApplicationResults;

namespace Drosy.Application.UseCases.Plans.Interfaces;

/// <summary>
/// Defines the service layer contract for handling Plan-related operations.
/// </summary>
public interface IPlanService
{
    /// <summary>
    /// Creates a new plan with the specified data.
    /// </summary>
    /// <param name="newPlan">The data transfer object containing information about the new plan to be created.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing the created <see cref="PlanDto"/> if successful,
    /// or error details if creation fails.
    /// </returns>
    Task<Result<PlanDto>> CreatePlanAsync(CreatePlanDTo newPlan, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a plan by its unique identifier.
    /// </summary>
    /// <param name="id">The ID of the plan to retrieve.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing the <see cref="PlanDto"/> if found,
    /// or error information if the plan does not exist.
    /// </returns>
    Task<Result<PlanDto>> GetPlanByIdAsync(int id, CancellationToken cancellationToken);
}