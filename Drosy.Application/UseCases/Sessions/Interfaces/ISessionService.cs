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
        /// Get all sessions  on a specific date.
        /// </summary>
        Task<Result<DataResult<SessionDTO>>> GetSessionsByDate(DateTime date, CancellationToken ct);
        /// <summary>
        /// Get all sessions for a given plan between two dates (inclusive).
        /// </summary>
        Task<Result<DataResult<SessionDTO>>> GetSessionsInRange( DateTime start, DateTime end, CancellationToken ct);
        /// <summary>
        /// Get all sessions for a given plan within the week that contains the anchor date.
        /// </summary>
        Task<Result<DataResult<SessionDTO>>> GetSessionsByWeek( int year, int weekNumber, CancellationToken ct);
        /// <summary>
        /// Get all sessions for a given plan within the month that contains the anchor date.
        /// </summary>
        Task<Result<DataResult<SessionDTO>>> GetSessionsByMonth( int year, int month, CancellationToken ct);
        /// <summary>
        /// Get all sessions for a given plan that match a particular status.
        /// </summary>
        Task<Result<DataResult<SessionDTO>>> GetSessionsByStatus( SessionStatus status, CancellationToken ct);

        Task<Result<DataResult<SessionDTO>>> GetSessionsByDate(int planId, DateTime date, CancellationToken ct);
     
        Task<Result<DataResult<SessionDTO>>> GetSessionsInRange(int planId, DateTime start, DateTime end, CancellationToken ct);
    
        Task<Result<DataResult<SessionDTO>>> GetSessionsByWeek(int planId, int year, int weekNumber, CancellationToken ct);
        
        Task<Result<DataResult<SessionDTO>>> GetSessionsByMonth(int planId, int year, int month, CancellationToken ct);
    
        Task<Result<DataResult<SessionDTO>>> GetSessionsByStatus(int planId, SessionStatus status, CancellationToken ct);
        Task<Result<DataResult<SessionDTO>>> GetSessionsByPlan(int planId, CancellationToken ct);
        
        /// <summary>
        /// Retrieves a session by its unique identifier.
        /// </summary>
        /// <param name="id">The ID of the session.</param>
        /// <param name="cancellationToken">Token for cancelling the asynchronous operation.</param>
        /// <returns>A result containing the session DTO if found, otherwise an error result.</returns>
        Task<Result<SessionDTO>> GetByIdAsync(int id, CancellationToken cancellationToken);

        #endregion

        #region Write

        /// <summary>
        /// Creates a new session using the provided session data.
        /// </summary>
        /// <param name="sessionDTO">The DTO containing session creation details.</param>
        /// <param name="cancellationToken">Token for cancelling the asynchronous operation.</param>
        /// <returns>A result containing the newly created session DTO or an error result.</returns>
        Task<Result<SessionDTO>> CreateAsync(CreateSessionDTO sessionDTO, CancellationToken cancellationToken);

        /// <summary>
        /// Reschedules an existing session in case of emergencies. Ensures no timing conflicts or plan disruptions.
        /// </summary>
        /// <param name="sessionId">The ID of the session to reschedule.</param>
        /// <param name="dto">DTO containing the new date, time, and reason for rescheduling.</param>
        /// <param name="cancellationToken">Token for cancelling the asynchronous operation.</param>
        /// <returns>A result containing the updated session DTO or an appropriate failure message.</returns>
        Task<Result<SessionDTO>> RescheduleAsync(int sessionId, RescheduleSessionDTO dto, CancellationToken cancellationToken);

        #endregion
    }
}
