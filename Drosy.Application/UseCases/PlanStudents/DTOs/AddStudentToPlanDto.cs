namespace Drosy.Application.UseCases.PlanStudents.DTOs
{
    public class AddStudentToPlanDto
    {
        public int StudentId { get; set; }
        public string Notes { get; set; } = null!;
    }

}
