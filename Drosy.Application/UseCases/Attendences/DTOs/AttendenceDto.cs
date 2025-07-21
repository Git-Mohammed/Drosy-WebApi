using Drosy.Application.UseCases.Students.DTOs;
using Drosy.Domain.Enums;

namespace Drosy.Application.UseCases.Attendences.DTOs
{
    public class AttendenceDto()
    {
        public int SessionId { get; set; }
        public int StudentId { get; set; }
        public AttendenceStatus Status { get; set; }
        public string Note { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        #region Nav Properties
        public StudentDTO Student { get; set; } = null!;
        public SessionDto Session { get; set; } = null!;

        #endregion
    }
    public class SessionDto
    {

    }
}

