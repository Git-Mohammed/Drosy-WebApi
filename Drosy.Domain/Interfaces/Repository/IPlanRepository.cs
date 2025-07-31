using Drosy.Domain.Entities;
using Drosy.Domain.Enums;
using Drosy.Domain.Interfaces.Common.Repository;

namespace Drosy.Domain.Interfaces.Repository;

/// <summary>
/// Interface for managing Plan-related data access operations.
/// Inherits from the base repository interface for generic CRUD operations.
/// </summary>
public interface IPlanRepository : IRepository<Plan>
{
    #region Read

    /// <summary>
    /// Checks if a plan with the specified ID exists.
    /// </summary>
    /// <param name="id">The ID of the plan to check.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>
    /// A task that returns <c>true</c> if the plan exists; otherwise, <c>false</c>.
    /// </returns>
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken);

    /// <summary>
    /// Checks if there is any active plan that conflicts with the specified session time range.
    /// </summary>
    /// <param name="day">A list of <see cref="PlanDay"/> objects defining the session time range.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>
    /// A task that returns <c>true</c> if a conflicting session exists; otherwise, <c>false</c>.
    /// </returns>
    Task<bool> ExistsAsync(List<PlanDay> day, CancellationToken cancellationToken);

    Task<Plan?> GetByIdAsync(int planId, CancellationToken ct);

    /// <summary>
    /// Retrieves all plans scheduled on a specific date.
    /// </summary>
    /// <param name="date">The date to filter plans.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A collection of plans scheduled on the specified date.</returns>
    Task<IEnumerable<Plan>> GetAllByDateAsync(DateTime date, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves all plans scheduled within a specific date range.
    /// </summary>
    /// <param name="start">Start of the date range.</param>
    /// <param name="end">End of the date range.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A collection of plans within the specified range.</returns>
    Task<IEnumerable<Plan>> GetAllInRangeAsync(DateTime start, DateTime end, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves all plans with the specified status.
    /// </summary>
    /// <param name="status">The status to filter plans by.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A collection of plans matching the given status.</returns>
    Task<IEnumerable<Plan>> GetAllByStatusAsync(PlanStatus status, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves all plans of a specific type.
    /// </summary>
    /// <param name="type">The type of the plan to filter by.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A collection of plans matching the specified type.</returns>
    Task<IEnumerable<Plan>> GetAllByTypeAsync(PlanTypes type, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves all plans scheduled within a specific week of a given year.
    /// </summary>
    /// <param name="year">The calendar year.</param>
    /// <param name="week">The ISO 8601 week number.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A collection of plans scheduled during the specified week.</returns>
    Task<IEnumerable<Plan>> GetAllByWeekAsync(int year, int week, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves all plans scheduled within a specific month of a given year.
    /// </summary>
    /// <param name="year">The calendar year.</param>
    /// <param name="month">The month number (1–12).</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A collection of plans scheduled during the specified month.</returns>
    Task<IEnumerable<Plan>> GetAllByMonthAsync(int year, int month, CancellationToken cancellationToken);


    Task<IEnumerable<Plan>> GetAllWithDetailsAsync(CancellationToken ct);
    Task<IEnumerable<Plan>> GetAllWithDetailsByStatusAsync(PlanStatus status, CancellationToken ct);
    
    #endregion

    #region Write
    // Add write-related documentation and methods here when applicable.
    #endregion
}
