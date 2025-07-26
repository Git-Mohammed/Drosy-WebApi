using Drosy.Application.UseCases.Sessions.DTOs;
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
