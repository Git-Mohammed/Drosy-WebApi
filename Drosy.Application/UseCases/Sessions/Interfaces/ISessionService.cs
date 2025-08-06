using Drosy.Application.UseCases.Sessions.DTOs;
using Drosy.Domain.Enums;
using Drosy.Domain.Shared.ApplicationResults;

namespace Drosy.Application.UseCases.Sessions.Interfaces
{
    /// <summary>
    /// Defines the core operations related to session management including retrieval,
    /// creation, and rescheduling with validation and result encapsulation.
    /// </summary>
    public interface ISessionService
    {
        #region Read

        /// <summary>
        /// Retrieves a session by its unique identifier.
        /// </summary>
        /// <param name="id">The ID of the session.</param>
        /// <param name="cancellationToken">Token for cancelling the asynchronous operation.</param>
        /// <returns>A result containing the session DTO if found, otherwise an error result.</returns>
        Task<Result<SessionDTO>> GetByIdAsync(int id, CancellationToken ct);

        #region Non-plan methods

        /// <summary>
        /// Gets all sessions.
        /// </summary>
        /// <param name="ct">Cancellation token to cancel the request.</param>
        /// <returns>A result containing a list of session DTOs.</returns>
        Task<Result<DataResult<SessionDTO>>> GetAllAsync(CancellationToken ct);

        /// <summary>
        /// Gets all sessions on a specific date.
        /// </summary>
        /// <param name="date">The date to filter sessions by.</param>
        /// <param name="ct">Cancellation token to cancel the request.</param>
        /// <returns>A result containing a list of session DTOs.</returns>
        Task<Result<DataResult<SessionDTO>>> GetSessionsByDate(DateTime date, CancellationToken ct);

        /// <summary>
        /// Gets all sessions between two dates (inclusive).
        /// </summary>
        /// <param name="start">Start of the date range.</param>
        /// <param name="end">End of the date range.</param>
        /// <param name="ct">Cancellation token to cancel the request.</param>
        /// <returns>A result containing a list of session DTOs.</returns>
        Task<Result<DataResult<SessionDTO>>> GetSessionsInRange(DateTime start, DateTime end, CancellationToken ct);

        /// <summary>
        /// Gets all sessions within a specific week.
        /// </summary>
        /// <param name="year">The year of the week.</param>
        /// <param name="weekNumber">The ISO 8601 week number.</param>
        /// <param name="ct">Cancellation token to cancel the request.</param>
        /// <returns>A result containing a list of session DTOs.</returns>
        Task<Result<DataResult<SessionDTO>>> GetSessionsByWeek(int year, int weekNumber, CancellationToken ct);

        /// <summary>
        /// Gets all sessions within a specific month.
        /// </summary>
        /// <param name="year">The year of the month.</param>
        /// <param name="month">The month number (1-12).</param>
        /// <param name="ct">Cancellation token to cancel the request.</param>
        /// <returns>A result containing a list of session DTOs.</returns>
        Task<Result<DataResult<SessionDTO>>> GetSessionsByMonth(int year, int month, CancellationToken ct);

        /// <summary>
        /// Gets all sessions with a specific status.
        /// </summary>
        /// <param name="status">The status to filter sessions by.</param>
        /// <param name="ct">Cancellation token to cancel the request.</param>
        /// <returns>A result containing a list of session DTOs.</returns>
        Task<Result<DataResult<SessionDTO>>> GetSessionsByStatus(SessionStatus status, CancellationToken ct);
        #endregion



        #region Calender
        Task<Result<DataResult<CalenderSessionDto>>> GetSessionsCalenderAsync(CancellationToken ct);

        Task<Result<DataResult<CalenderSessionDto>>> GetSessionsCalenderByDate(DateTime date, CancellationToken ct);

        Task<Result<DataResult<CalenderSessionDto>>> GetSessionsCalenderInRange(DateTime start, DateTime end, CancellationToken ct);

        Task<Result<DataResult<CalenderSessionDto>>> GetSessionsCalenderByWeek(int year, int weekNumber, CancellationToken ct);

        Task<Result<DataResult<CalenderSessionDto>>> GetSessionsCalenderByMonth(int year, int month, CancellationToken ct);

        Task<Result<DataResult<CalenderSessionDto>>> GetSessionsCalenderByStatus(PlanStatus status, CancellationToken ct);
        #endregion


        #region Plan-scoped methods
        /// <summary>
        /// Gets all sessions for a specific plan on a specific date.
        /// </summary>
        /// <param name="planId">The ID of the plan.</param>
        /// <param name="date">The date to filter sessions by.</param>
        /// <param name="ct">Cancellation token to cancel the request.</param>
        /// <returns>A result containing a list of session DTOs.</returns>
        Task<Result<DataResult<SessionDTO>>> GetSessionsByDate(int planId, DateTime date, CancellationToken ct);

        /// <summary>
        /// Gets all sessions for a specific plan within a date range.
        /// </summary>
        /// <param name="planId">The ID of the plan.</param>
        /// <param name="start">Start of the date range.</param>
        /// <param name="end">End of the date range.</param>
        /// <param name="ct">Cancellation token to cancel the request.</param>
        /// <returns>A result containing a list of session DTOs.</returns>
        Task<Result<DataResult<SessionDTO>>> GetSessionsInRange(int planId, DateTime start, DateTime end, CancellationToken ct);

        /// <summary>
        /// Gets all sessions for a specific plan within a specific week.
        /// </summary>
        /// <param name="planId">The ID of the plan.</param>
        /// <param name="year">The year of the week.</param>
        /// <param name="weekNumber">The ISO 8601 week number.</param>
        /// <param name="ct">Cancellation token to cancel the request.</param>
        /// <returns>A result containing a list of session DTOs.</returns>
        Task<Result<DataResult<SessionDTO>>> GetSessionsByWeek(int planId, int year, int weekNumber, CancellationToken ct);

        /// <summary>
        /// Gets all sessions for a specific plan within a specific month.
        /// </summary>
        /// <param name="planId">The ID of the plan.</param>
        /// <param name="year">The year of the month.</param>
        /// <param name="month">The month number (1-12).</param>
        /// <param name="ct">Cancellation token to cancel the request.</param>
        /// <returns>A result containing a list of session DTOs.</returns>
        Task<Result<DataResult<SessionDTO>>> GetSessionsByMonth(int planId, int year, int month, CancellationToken ct);

        /// <summary>
        /// Gets all sessions for a specific plan with a specific status.
        /// </summary>
        /// <param name="planId">The ID of the plan.</param>
        /// <param name="status">The session status to filter by.</param>
        /// <param name="ct">Cancellation token to cancel the request.</param>
        /// <returns>A result containing a list of session DTOs.</returns>
        Task<Result<DataResult<SessionDTO>>> GetSessionsByStatus(int planId, SessionStatus status, CancellationToken ct);

        /// <summary>
        /// Gets all sessions for a specific plan regardless of date or status.
        /// </summary>
        /// <param name="planId">The ID of the plan.</param>
        /// <param name="ct">Cancellation token to cancel the request.</param>
        /// <returns>A result containing a list of session DTOs.</returns>
        Task<Result<DataResult<SessionDTO>>> GetSessionsByPlan(int planId, CancellationToken ct);
        #endregion



        #endregion

        #region Write

        /// <summary>
        /// Creates a new session using the provided session data.
        /// </summary>
        /// <param name="sessionDTO">The DTO containing session creation details.</param>
        /// <param name="cancellationToken">Token for cancelling the asynchronous operation.</param>
        /// <returns>A result containing the newly created session DTO or an error result.</returns>
        Task<Result<SessionDTO>> CreateAsync(CreateSessionDTO sessionDTO, CancellationToken ct);

        /// <summary>
        /// Reschedules an existing session in case of emergencies. Ensures no timing conflicts or plan disruptions.
        /// </summary>
        /// <param name="sessionId">The ID of the session to reschedule.</param>
        /// <param name="dto">DTO containing the new date, time, and reason for rescheduling.</param>
        /// <param name="cancellationToken">Token for cancelling the asynchronous operation.</param>
        /// <returns>A result containing the updated session DTO or an appropriate failure message.</returns>
        Task<Result<SessionDTO>> RescheduleAsync(int sessionId, RescheduleSessionDTO dto, CancellationToken ct);

        #endregion
    }
}
