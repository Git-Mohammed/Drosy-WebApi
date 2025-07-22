using Drosy.Domain.Entities;
using Drosy.Domain.Interfaces.Common.Repository;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Drosy.Domain.Interfaces.Repository
{
    /// <summary>
    /// Defines data access operations for Session entities.
    /// </summary>
    public interface ISessionRepository : IRepository<Session>
    {
        #region 🔍 Single Retrieval

        /// <summary>
        /// Retrieves a session entity by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the session.</param>
        /// <param name="cancellationToken">Token to cancel the async operation.</param>
        /// <returns>The session entity if found; otherwise, null.</returns>
        Task<Session?> GetByIdAsync(int id, CancellationToken cancellationToken);

        #endregion

        #region 📅 Session Queries

        /// <summary>
        /// Retrieves all sessions scheduled for a specific date and plan.
        /// </summary>
        /// <param name="date">The target date to filter sessions.</param>
        /// <param name="planId">The associated plan identifier.</param>
        /// <param name="cancellationToken">Token to cancel the async operation.</param>
        /// <returns>A list of matching session entities.</returns>
        Task<IEnumerable<Session>> GetSessionsByDateAndPlanAsync(DateTime date, int planId, CancellationToken cancellationToken);

        /// <summary>
        /// Checks whether any session overlaps with a given time range for a specific date and plan.
        /// </summary>
        /// <param name="date">The date to check for overlapping sessions.</param>
        /// <param name="planId">The associated plan identifier.</param>
        /// <param name="startTime">The proposed start time of the session.</param>
        /// <param name="endTime">The proposed end time of the session.</param>
        /// <param name="cancellationToken">Token to cancel the async operation.</param>
        /// <returns>True if an overlapping session exists; otherwise, false.</returns>
        Task<bool> SessionExistsAsync(DateTime date, int planId, DateTime startTime, DateTime endTime, CancellationToken cancellationToken);

        #endregion
    }
}
