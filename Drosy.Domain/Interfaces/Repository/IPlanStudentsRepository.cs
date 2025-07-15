using Drosy.Domain.Entities;
using Drosy.Domain.Interfaces.Common.Repository;
using System.Linq.Expressions;

namespace Drosy.Domain.Interfaces.Repository
{
    /// <summary>
    /// Provides specialized operations for managing <see cref="PlanStudent"/> entities.
    /// </summary>
    public interface IPlanStudentsRepository : IRepository<PlanStudent>
    {
        /// <summary>
        /// Checks whether any <see cref="PlanStudent"/> exists that matches the given predicate.
        /// </summary>
        /// <param name="predicate">The condition to match.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns><c>true</c> if any matching entity exists; otherwise, <c>false</c>.</returns>
        Task<bool> ExistsAsync(int planId,int studentId, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves a list of student IDs that are already assigned to the specified plan.
        /// </summary>
        /// <param name="planId">The ID of the plan to check.</param>
        /// <param name="studentIds">A list of student IDs to filter.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        /// <returns>A list of student IDs that are part of the specified plan.</returns>
        Task<List<int>> GetStudentIdsInPlanAsync(int planId, List<int> studentIds, CancellationToken ct);
    }
}
