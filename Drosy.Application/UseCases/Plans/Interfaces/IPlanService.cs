using Drosy.Application.UseCases.Plans.DTOs;
using Drosy.Application.UseCases.Schedule.DTOs;
using Drosy.Domain.Entities;
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
    /// A <see cref="Result"/> indicating success if the plan exists, or failure with error information.

    /// </returns>
    Task<Result> ExistsAsync(int id, CancellationToken cancellationToken);

    /// <summary>

    /// Retrieves all available plans.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing a collection of <see cref="PlanDto"/> objects.
    /// </returns>
    Task<Result<DataResult<PlanDto>>> GetAllAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves all plans scheduled on a specific date.
    /// </summary>
    /// <param name="date">The date to filter plans by.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing matching <see cref="PlanDto"/> entries.
    /// </returns>
    Task<Result<DataResult<PlanDto>>> GetPlansByDate(DateTime date, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves all plans within a specified date range.
    /// </summary>
    /// <param name="start">Start date of the range.</param>
    /// <param name="end">End date of the range.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing plans in the specified date range.
    /// </returns>
    Task<Result<DataResult<PlanDto>>> GetPlansInRange(DateTime start, DateTime end, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves all plans with a specified status.
    /// </summary>
    /// <param name="status">The status to filter plans by.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing plans with the given status.
    /// </returns>
    Task<Result<DataResult<PlanDto>>> GetPlansByStatus(PlanStatus status, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves all plans of a specific type.
    /// </summary>
    /// <param name="type">The type of plan to filter by.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing matching plans.
    /// </returns>
    Task<Result<DataResult<PlanDto>>> GetPlansByType(PlanTypes type, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves all plans scheduled in a specific week of a given year.
    /// </summary>
    /// <param name="year">The year to search in.</param>
    /// <param name="week">The ISO 8601 week number.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing plans scheduled in that week.
    /// </returns>
    Task<Result<DataResult<PlanDto>>> GetPlansByWeek(int year, int week, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves all plans scheduled in a specific month of a given year.
    /// </summary>
    /// <param name="year">The year to search in.</param>
    /// <param name="month">The month (1–12).</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing plans from the specified month.
    /// </returns>
    Task<Result<DataResult<PlanDto>>> GetPlansByMonth(int year, int month, CancellationToken cancellationToken);


    Task<Result<List<Plan>>> GetAllPlansWithDetailsAsync(CancellationToken ct);
    Task<Result<List<Plan>>> GetPlansWithDetailsByStatusAsync(PlanStatus status, CancellationToken ct);

    #region Calender
    Task<Result<DataResult<CalenderSessionDto>>> GetPlanSessionsCalenderAsync(int planId, CancellationToken ct);
    Task<Result<DataResult<CalenderSessionDto>>> GetPlanSessionsCalenderByDateAsync(int planId, DateTime date, CancellationToken ct);
    Task<Result<DataResult<CalenderSessionDto>>> GetPlanSessionsCalenderByRangeAsync(int planId, DateTime start, DateTime end, CancellationToken ct);
    Task<Result<DataResult<CalenderSessionDto>>> GetPlanSessionsCalenderByStatusAsync(int planId, PlanStatus status, CancellationToken ct);
    Task<Result<DataResult<CalenderSessionDto>>> GetPlanSessionsCalenderByWeekAsync(int planId, int year, int week, CancellationToken ct);
    Task<Result<DataResult<CalenderSessionDto>>> GetPlanSessionsCalenderByMonthAsync(int planId, int year, int month, CancellationToken ct);
    Task<List<CalenderSessionDto>> GenerateSessionsForPlanAsync(Plan plan, DateTime? startFilter, DateTime? endFilter, CancellationToken ct);

    #endregion


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
