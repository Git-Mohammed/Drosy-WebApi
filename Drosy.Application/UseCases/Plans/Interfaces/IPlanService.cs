using Drosy.Application.UseCases.Plans.DTOs;
using Drosy.Domain.Enums;
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
    Task<Result<PlanDto>> CreatePlanAsync(CreatePlanDto newPlan, CancellationToken cancellationToken);

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
    /// <summary>
    /// Retrieves all available plans with their details.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>
    /// A <see cref="Result"/> containing a <see cref="DataResult{T}"/> of a list of <see cref="PlanDto"/> objects,
    /// or a failure result if no plans are found.
    /// </returns>
    Task<Result<DataResult<PlanDto>>> GetAllPlansAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Checks whether a plan with the specified ID exists.
    /// </summary>
    /// <param name="id">The ID of the plan to check.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>
    /// A <see cref="Result"/> indicating whether the plan exists or not.
    /// </returns>
    Task<Result> ExistsAsync(int id, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes the plan with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the plan to delete.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>
    /// A <see cref="Result"/> indicating success or failure of the deletion.
    /// </returns>
    Task<Result> DeleteAsync(int id, CancellationToken cancellationToken);

    /// <summary>
    /// Updates the status of the plan with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the plan to update.</param>
    /// <param name="status">The new status to apply to the plan.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>
    /// A <see cref="Result"/> indicating success or failure of the update.
    /// </returns>
    Task<Result> UpdateStatusAsync(int id, UpdatePlanStatusDto status, CancellationToken cancellationToken);
}