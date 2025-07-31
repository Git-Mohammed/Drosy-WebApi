namespace Drosy.Application.UseCases.PlanStudents.DTOs;

public class CalenderPlanStudentDto
{
    public int StudentId { get; set; }
    public string Notes { get; set; } = null!;
    public decimal Fee { get; set; }
    public DateTime CreatedAt { get; set; }

    // studnet info
    public string FullName { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
}
