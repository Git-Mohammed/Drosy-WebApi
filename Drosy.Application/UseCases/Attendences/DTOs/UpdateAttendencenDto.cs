using Drosy.Domain.Enums;

namespace Drosy.Application.UseCases.Attendences.DTOs
{
    /// <summary>
    /// Data Transfer Object used to update an existing attendance record.
    /// Contains the attendance status and an optional note.
    /// </summary>
    public class UpdateAttendencenDto
    {
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
