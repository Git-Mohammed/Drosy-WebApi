namespace Drosy.Application.UseCases.Plans.DTOs;

public class PlanDayDto
{
    public string Day { get; set; }
    public TimeSpan StartSession { get; set; }
    public TimeSpan EndSession { get; set; }
}
