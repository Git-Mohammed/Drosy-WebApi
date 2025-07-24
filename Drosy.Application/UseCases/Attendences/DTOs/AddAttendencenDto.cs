using Drosy.Domain.Enums;

namespace Drosy.Application.UseCases.Attendences.DTOs
{
    /// <summary>
    /// Data Transfer Object used to add a new attendance record.
    /// Contains information about the student, attendance status, and an optional note.
    /// </summary>
    public class AddAttendencenDto
    {
        /// <summary>
        /// Gets or sets the identifier of the student for whom the attendance is being recorded.
        /// </summary>
        public int StudentId { get; set; }

        /// <summary>
        /// Gets or sets the attendance status (e.g., Present or Absent).
        /// </summary>
        public AttendenceStatus Status { get; set; }

        /// <summary>
        /// Gets or sets an optional note related to the attendance.
        /// </summary>
        public string? Note { get; set; } = null!;
    }
}
