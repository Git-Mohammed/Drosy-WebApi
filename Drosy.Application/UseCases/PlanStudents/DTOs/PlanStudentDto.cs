using Drosy.Application.UseCases.Students.DTOs;

namespace Drosy.Application.UseCases.PlanStudents.DTOs
{
    public class PlanStudentDto
    {
        public int PlanId { get; set; }
        public int StudentId { get; set; }
        public string Notes { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        #region Nav Properties
        public StudentDTO Student { get; set; } = null!;
        #endregion
    }

}
