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
        /// Checks whether any session overlaps with a specified time range on a given date and plan.
        /// </summary>
        Task<bool> ExistsAsync(DateTime date, int planId, DateTime startTime, DateTime endTime, CancellationToken cancellationToken);

        /// <summary>
        /// Checks whether any session overlaps with a specified time range on a given date and plan,
        /// excluding the session with the provided identifier.
        /// </summary>
        Task<bool> ExistsAsync(int excludeSessionId, DateTime date, DateTime startTime, DateTime endTime, CancellationToken cancellationToken);

        /// <summary>
        /// Checks whether any session overlaps with a specified time range on a given date, regardless of plan.
        /// </summary>
        Task<bool> ExistsAsync(DateTime date, DateTime startTime, DateTime endTime, CancellationToken cancellationToken);

        // Non-plan scoped queries
        Task<IEnumerable<Session>> GetSessionsByDateAsync(DateTime date, CancellationToken cancellationToken);
        Task<IEnumerable<Session>> GetSessionsInRangeAsync(DateTime start, DateTime end, CancellationToken cancellationToken);
        Task<IEnumerable<Session>> GetSessionsByWeekAsync(int year, int weekNumber, CancellationToken cancellationToken);
        Task<IEnumerable<Session>> GetSessionsByMonthAsync(int year, int month, CancellationToken cancellationToken);
        Task<IEnumerable<Session>> GetSessionsByStatusAsync(SessionStatus status, CancellationToken cancellationToken);

        // Plan-scoped queries
        Task<IEnumerable<Session>> GetByPlanAsync(int planId, CancellationToken cancellationToken);
        Task<IEnumerable<Session>> GetByPlanAndDateAsync(int planId, DateTime date, CancellationToken cancellationToken);
        Task<IEnumerable<Session>> GetByPlanAndRangeAsync(int planId, DateTime start, DateTime end, CancellationToken cancellationToken);
        Task<IEnumerable<Session>> GetByPlanAndWeekAsync(int planId, int year, int weekNumber, CancellationToken cancellationToken);
        Task<IEnumerable<Session>> GetByPlanAndMonthAsync(int planId, int year, int month, CancellationToken cancellationToken);
        Task<IEnumerable<Session>> GetByPlanAndStatusAsync(int planId, SessionStatus status, CancellationToken cancellationToken);

        #endregion

        #region Write 
       
        #endregion
    }
}
