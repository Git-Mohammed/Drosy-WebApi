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
        Task<bool> ExistsAsync(DateTime date, int planId, DateTime startTime, DateTime endTime, CancellationToken ct);

        /// <summary>
        /// Checks whether any session overlaps with a specified time range on a given date and plan,
        /// excluding the session with the provided identifier.
        /// </summary>
        Task<bool> ExistsAsync(int excludeSessionId, DateTime date, DateTime startTime, DateTime endTime, CancellationToken ct);

        /// <summary>
        /// Checks whether any session overlaps with a specified time range on a given date, regardless of plan.
        /// </summary>
        Task<bool> ExistsAsync(DateTime date, DateTime startTime, DateTime endTime, CancellationToken ct);

        // Non-plan scoped queries
        /// <summary>
        /// Retrieves all sessions .
        /// </summary>
        /// <param name="cancellationToken">Token to cancel the async operation.</param>
        Task<IEnumerable<Session>> GetAllSessionsAsync(DateTime date, CancellationToken ct);

        /// <summary>
        /// Retrieves all sessions occurring on a specific date.
        /// </summary>
        /// <param name="date">The date to search for sessions.</param>
        /// <param name="cancellationToken">Token to cancel the async operation.</param>
        Task<IEnumerable<Session>> GetSessionsByDateAsync(DateTime date, CancellationToken ct);

        /// <summary>
        /// Retrieves all sessions occurring within a specified date range.
        /// </summary>
        /// <param name="start">Start of the date range.</param>
        /// <param name="end">End of the date range.</param>
        /// <param name="cancellationToken">Token to cancel the async operation.</param>
        Task<IEnumerable<Session>> GetSessionsInRangeAsync(DateTime start, DateTime end, CancellationToken ct);

        /// <summary>
        /// Retrieves all sessions occurring within a specific week of a given year.
        /// </summary>
        /// <param name="year">The calendar year.</param>
        /// <param name="weekNumber">The ISO 8601 week number.</param>
        /// <param name="cancellationToken">Token to cancel the async operation.</param>
        Task<IEnumerable<Session>> GetSessionsByWeekAsync(int year, int weekNumber, CancellationToken ct);

        /// <summary>
        /// Retrieves all sessions scheduled within a specific month of a given year.
        /// </summary>
        /// <param name="year">The calendar year.</param>
        /// <param name="month">The month (1–12).</param>
        /// <param name="cancellationToken">Token to cancel the async operation.</param>
        Task<IEnumerable<Session>> GetSessionsByMonthAsync(int year, int month, CancellationToken ct);

        /// <summary>
        /// Retrieves all sessions with the specified status.
        /// </summary>
        /// <param name="status">The status of the sessions to retrieve.</param>
        /// <param name="cancellationToken">Token to cancel the async operation.</param>
        Task<IEnumerable<Session>> GetSessionsByStatusAsync(SessionStatus status, CancellationToken ct);

        // Plan-scoped queries

        /// <summary>
        /// Retrieves all sessions associated with a specific plan.
        /// </summary>
        /// <param name="planId">The unique identifier of the plan.</param>
        /// <param name="cancellationToken">Token to cancel the async operation.</param>
        Task<IEnumerable<Session>> GetByPlanAsync(int planId, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves all sessions for a given plan on a specific date.
        /// </summary>
        /// <param name="planId">The unique identifier of the plan.</param>
        /// <param name="date">The date to filter sessions.</param>
        /// <param name="cancellationToken">Token to cancel the async operation.</param>
        Task<IEnumerable<Session>> GetByPlanAndDateAsync(int planId, DateTime date, CancellationToken ct);

        /// <summary>
        /// Retrieves all sessions for a given plan within a specified date range.
        /// </summary>
        /// <param name="planId">The unique identifier of the plan.</param>
        /// <param name="start">Start of the date range.</param>
        /// <param name="end">End of the date range.</param>
        /// <param name="cancellationToken">Token to cancel the async operation.</param>
        Task<IEnumerable<Session>> GetByPlanAndRangeAsync(int planId, DateTime start, DateTime end, CancellationToken ct);

        /// <summary>
        /// Retrieves all sessions for a specific plan during a specific week of the year.
        /// </summary>
        /// <param name="planId">The unique identifier of the plan.</param>
        /// <param name="year">The calendar year.</param>
        /// <param name="weekNumber">The ISO 8601 week number.</param>
        /// <param name="cancellationToken">Token to cancel the async operation.</param>
        Task<IEnumerable<Session>> GetByPlanAndWeekAsync(int planId, int year, int weekNumber, CancellationToken ct);

        /// <summary>
        /// Retrieves all sessions for a specific plan during a specific month of the year.
        /// </summary>
        /// <param name="planId">The unique identifier of the plan.</param>
        /// <param name="year">The calendar year.</param>
        /// <param name="month">The month (1–12).</param>
        /// <param name="cancellationToken">Token to cancel the async operation.</param>
        Task<IEnumerable<Session>> GetByPlanAndMonthAsync(int planId, int year, int month, CancellationToken ct);

        /// <summary>
        /// Retrieves all sessions for a given plan with a specific status.
        /// </summary>
        /// <param name="planId">The unique identifier of the plan.</param>
        /// <param name="status">The status to filter sessions.</param>
        /// <param name="cancellationToken">Token to cancel the async operation.</param>
        Task<IEnumerable<Session>> GetByPlanAndStatusAsync(int planId, SessionStatus status, CancellationToken ct);

        #endregion

        #region Write 
        // Write operations can be added here.
        #endregion
    }
}
