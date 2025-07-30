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
    /// <summary>




    #region Read
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
    /// <param name="day"></param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>
    /// A task that returns <c>true</c> if a conflicting session exists; otherwise, <c>false</c>.
    /// </returns>
    Task<bool> ExistsAsync(List<PlanDay> day, CancellationToken cancellationToken);
    /// <summary>
    /// Retrieves a plan by its unique identifier.
    /// </summary>
    /// <param name="id">The ID of the plan.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>
    /// A task that returns the <see cref="Plan"/> if found; otherwise, <c>null</c>.
    /// </returns>
    Task<Plan?> GetByIdAsync(int id, CancellationToken cancellationToken);

    // New filtering repo methods
    Task<IEnumerable<Plan>> GetByDateAsync(DateTime date, CancellationToken cancellationToken);
    Task<IEnumerable<Plan>> GetInRangeAsync(DateTime start, DateTime end, CancellationToken cancellationToken);
    Task<IEnumerable<Plan>> GetByStatusAsync(PlanStatus status, CancellationToken cancellationToken);
    Task<IEnumerable<Plan>> GetByTypeAsync(PlanTypes type, CancellationToken cancellationToken);
    Task<IEnumerable<Plan>> GetByWeekAsync(int year, int week, CancellationToken cancellationToken);
    Task<IEnumerable<Plan>> GetByMonthAsync(int year, int month, CancellationToken cancellationToken);
    #endregion


    #region Write
    #endregion
}