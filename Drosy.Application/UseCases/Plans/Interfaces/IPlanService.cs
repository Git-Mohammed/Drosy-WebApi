using Drosy.Application.UseCases.Plans.DTOs;
using Drosy.Domain.Enums;
using Drosy.Domain.Shared.ApplicationResults;

namespace Drosy.Application.UseCases.Plans.Interfaces;

/// <summary>
/// Defines the service layer contract for handling Plan-related operations.
/// </summary>
public interface IPlanService
{
    #region Read
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

    Task<Result> ExistsAsync(int id, CancellationToken cancellationToken);

    // Calendar and filter methods
    Task<Result<DataResult<PlanDto>>> GetAllAsync(CancellationToken cancellationToken);
    Task<Result<DataResult<PlanDto>>> GetPlansByDate(DateTime date, CancellationToken cancellationToken);
    Task<Result<DataResult<PlanDto>>> GetPlansInRange(DateTime start, DateTime end, CancellationToken cancellationToken);
    Task<Result<DataResult<PlanDto>>> GetPlansByStatus(PlanStatus status, CancellationToken cancellationToken);
    Task<Result<DataResult<PlanDto>>> GetPlansByType(PlanTypes type, CancellationToken cancellationToken);
    Task<Result<DataResult<PlanDto>>> GetPlansByWeek(int year, int week, CancellationToken cancellationToken);
    Task<Result<DataResult<PlanDto>>> GetPlansByMonth(int year, int month, CancellationToken cancellationToken);
    #endregion

    #region Write
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

    #endregion


}