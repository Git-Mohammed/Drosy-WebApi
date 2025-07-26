using Drosy.Domain.Entities;
using Drosy.Domain.Enums;
using Drosy.Domain.Interfaces.Common.Repository;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Drosy.Domain.Interfaces.Repository
{
    /// <summary>
    /// Defines data access operations for session entities.
    /// </summary>
    public interface ISessionRepository : IRepository<Session>
    { 
        #region Read Operations

        /// <summary>
        /// Retrieves a session entity by its unique identifier.
        /// </summary>
        /// <param name="id">The session's unique identifier.</param>
        /// <param name="cancellationToken">Token to cancel the async operation.</param>
        /// <returns>The matching session if found; otherwise, null.</returns>
        Task<Session?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves all sessions scheduled for a given date and plan.
        /// </summary>
        /// <param name="date">Target date.</param>
        /// <param name="planId">Associated plan ID.</param>
        /// <param name="cancellationToken">Token to cancel the async operation.</param>
        /// <returns>A list of sessions scheduled for the specified date and plan.</returns>
        Task<IEnumerable<Session>> GetSessionsByDateAsync(
            int planId,
            DateTime date,
            CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves all sessions for a given plan between two dates (inclusive).
        /// <param name="start">Target start date.</param>
        /// <param name="end">Target end date.</param>
        /// <param name="planId">Associated plan ID.</param>
        /// <param name="cancellationToken">Token to cancel the async operation.</param>
        /// </summary>
        Task<IEnumerable<Session>> GetSessionsInRangeAsync(
            int planId,
            DateTime start,
            DateTime end,
            CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves all sessions for a given plan that have the specified status.
        /// <param name="status">Target status.</param>
        /// <param name="planId">Associated plan ID.</param>
        /// <param name="cancellationToken">Token to cancel the async operation.</param>
        /// </summary>
        Task<IEnumerable<Session>> GetSessionsByStatusAsync(
            int planId,
            SessionStatus status,
            CancellationToken cancellationToken);

        /// <summary>
        /// Checks whether any session overlaps with a specified time range on a given date and plan.
        /// </summary>
        /// <param name="date">Date to check.</param>
        /// <param name="planId">Associated plan ID.</param>
        /// <param name="startTime">Proposed start time.</param>
        /// <param name="endTime">Proposed end time.</param>
        /// <param name="cancellationToken">Token to cancel the async operation.</param>
        /// <returns>True if an overlap is found; otherwise, false.</returns>
        Task<bool> ExistsAsync(DateTime date, int planId, DateTime startTime, DateTime endTime, CancellationToken cancellationToken);

        /// <summary>
        /// Checks whether any session overlaps with a specified time range on a given date and plan,
        /// excluding the session with the provided identifier.
        /// </summary>
        /// <param name="excludeSessionId">Session ID to exclude from the check.</param>
        /// <param name="date">Date to check.</param>
        /// <param name="startTime">Proposed start time.</param>
        /// <param name="endTime">Proposed end time.</param>
        /// <param name="cancellationToken">Token to cancel the async operation.</param>
        /// <returns>True if another overlapping session exists; otherwise, false.</returns>
        Task<bool> ExistsAsync(int excludeSessionId, DateTime date, DateTime startTime, DateTime endTime,  CancellationToken cancellationToken);

        /// <summary>
        /// Checks whether any session overlaps with a specified time range on a given date, regardless of plan.
        /// </summary>
        /// <param name="date">Date to check.</param>
        /// <param name="startTime">Proposed start time.</param>
        /// <param name="endTime">Proposed end time.</param>
        /// <param name="cancellationToken">Token to cancel the async operation.</param>
        /// <returns>True if an overlap is found; otherwise, false.</returns>
        Task<bool> ExistsAsync(DateTime date, DateTime startTime, DateTime endTime, CancellationToken cancellationToken);

        #endregion

        #region Write Operations

        #endregion
    }
}
