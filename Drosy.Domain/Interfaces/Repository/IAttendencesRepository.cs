using Drosy.Domain.Entities;
using Drosy.Domain.Enums;
using Drosy.Domain.Interfaces.Common.Repository;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Drosy.Domain.Interfaces.Repository
{
    /// <summary>
    /// Repository interface for managing attendance entities.
    /// Provides methods for checking existence, retrieving, and querying attendances by session, student, and status.
    /// </summary>
    public interface IAttendencesRepository : IRepository<Attendence>
    {
        /// <summary>
        /// Checks if an attendance record exists for the specified session and student.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="studentId">The student identifier.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>True if the attendance exists; otherwise, false.</returns>
        Task<bool> ExistsAsync(int sessionId, int studentId, CancellationToken ct);

        /// <summary>
        /// Retrieves all attendance records for a list of students in a specified session.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="studentIds">The collection of student identifiers.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A collection of attendance records matching the students and session.</returns>
        Task<IEnumerable<Attendence>> GetAllForStudentBySessionAsync(int sessionId, IEnumerable<int> studentIds, CancellationToken ct);

        /// <summary>
        /// Retrieves an attendance record by session and student identifiers.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="studentId">The student identifier.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>The attendance entity if found; otherwise, null.</returns>
        Task<Attendence?> GetByIdAsync(int sessionId, int studentId, CancellationToken ct);

        /// <summary>
        /// Retrieves all attendance records for a given session.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A collection of attendance records for the session.</returns>
        Task<IEnumerable<Attendence>> GetAllForSessionAsync(int sessionId, CancellationToken ct);

        /// <summary>
        /// Retrieves all attendance records for a given session filtered by attendance status.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="status">The attendance status to filter by (e.g., Present or Absent).</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A collection of attendance records matching the session and status.</returns>
        Task<IEnumerable<Attendence>> GetAllForSessionByStatusAsync(int sessionId, AttendenceStatus status, CancellationToken ct);
    }
}
