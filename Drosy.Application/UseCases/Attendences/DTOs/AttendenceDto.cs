using Drosy.Application.UseCases.Sessions.DTOs;
using Drosy.Application.UseCases.Students.DTOs;
using Drosy.Domain.Enums;
using System;

namespace Drosy.Application.UseCases.Attendences.DTOs
{
    /// <summary>
    /// Data Transfer Object representing an attendance record.
    /// Contains information about session, student, attendance status, notes, and timestamps.
    /// </summary>
    public class AttendenceDto
    {
        /// <summary>
        /// Gets or sets the identifier of the session.
        /// </summary>
        public int SessionId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the student.
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

        /// <summary>
        /// Gets or sets the timestamp when the attendance record was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        #region Navigation Properties

        /// <summary>
        /// Gets or sets the student details associated with this attendance.
        /// </summary>
        public StudentDTO Student { get; set; } = null!;

        /// <summary>
        /// Gets or sets the session details associated with this attendance.
        /// </summary>
        public SessionDTO Session { get; set; } = null!;

        #endregion
    }
}
