using Drosy.Application.UseCases.Plans.DTOs;
using Drosy.Application.UseCases.PlanStudents.DTOs;

namespace Drosy.Application.UseCases.Sessions.DTOs;

public class CalenderSessionDto
{
    public int PlanId { get; set; }
    public string PlanType { get; set; } = null!;
    public string PlanStatus { get; set; } = null!;
    public DateTime ExcepectedDate { get; set; }
    public List<PlanDayDto> Days { get; set; } = null!;
    public int SessionPeriod { get; set; }
    public int Period { get; set; }
    public List<CalenderPlanStudentDto> Students { get; set; } = new();
}
