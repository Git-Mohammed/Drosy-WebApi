using Drosy.Application.UseCases.Attendences.DTOs;
using Drosy.Domain.Enums;
using Drosy.Domain.Shared.ApplicationResults;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Drosy.Application.UseCases.Attendences.Interfaces
{
    /// <summary>
    /// Service interface for managing attendences.
    /// Contains methods for retrieving, adding, updating, and deleting attendence records.
    /// </summary>
    public interface IAttendencesService
    {
        #region Read

        /// <summary>
        /// Retrieves an attendance record by session and student identifiers.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="studentId">The student identifier.</param>
        /// <param name="ct">Cancellation token to cancel the operation.</param>
        /// <returns>A Result wrapping the attendance data transfer object if found; otherwise, a failure result.</returns>
        Task<Result<AttendenceDto>> GetByIdAsync(int sessionId, int studentId, CancellationToken ct);

        /// <summary>
        /// Retrieves all attendance records for a given session.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="ct">Cancellation token to cancel the operation.</param>
        /// <returns>A Result wrapping a DataResult containing a list of attendance DTOs and total record count.</returns>
        Task<Result<DataResult<AttendenceDto>>> GetAllForSessionAsync(int sessionId, CancellationToken ct);

        /// <summary>
        /// Retrieves all attendance records for a given session filtered by attendance status.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="status">The attendance status to filter by (e.g., Present or Absent).</param>
        /// <param name="ct">Cancellation token to cancel the operation.</param>
        /// <returns>A Result wrapping a DataResult containing filtered attendance DTOs and total record count.</returns>
        Task<Result<DataResult<AttendenceDto>>> GetAllForSessionByStatusAsync(int sessionId, AttendenceStatus status, CancellationToken ct);

        #endregion

        #region Write

        /// <summary>
        /// Adds a new attendance record for a given session.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="dto">The attendance data transfer object containing student ID, status, and note.</param>
        /// <param name="ct">Cancellation token to cancel the operation.</param>
        /// <returns>A Result wrapping the newly added attendance DTO.</returns>
        Task<Result<AttendenceDto>> AddAsync(int sessionId, AddAttendencenDto dto, CancellationToken ct);

        /// <summary>
        /// Adds multiple attendance records for a given session.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="dto">A collection of attendance DTOs to add.</param>
        /// <param name="ct">Cancellation token to cancel the operation.</param>
        /// <returns>A Result wrapping a DataResult containing all added attendance DTOs.</returns>
        Task<Result<DataResult<AttendenceDto>>> AddRangeAsync(int sessionId, IEnumerable<AddAttendencenDto> dto, CancellationToken ct);

        /// <summary>
        /// Updates an existing attendance record identified by session and student IDs.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="studentId">The student identifier.</param>
        /// <param name="dto">The attendance update DTO containing new status and note.</param>
        /// <param name="ct">Cancellation token to cancel the operation.</param>
        /// <returns>A Result wrapping the updated attendance DTO.</returns>
        Task<Result<AttendenceDto>> UpdateAsync(int sessionId, int studentId, UpdateAttendencenDto dto, CancellationToken ct);

        /// <summary>
        /// Deletes an attendance record identified by session and student IDs.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="studentId">The student identifier.</param>
        /// <param name="ct">Cancellation token to cancel the operation.</param>
        /// <returns>A Result indicating success or failure of the deletion.</returns>
        Task<Result> DeleteAsync(int sessionId, int studentId, CancellationToken ct);

        #endregion
    }
}
