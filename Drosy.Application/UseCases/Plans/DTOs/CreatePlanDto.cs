
namespace Drosy.Application.UseCases.Plans.DTOs;

public class CreatePlanDto
{
    public string Type { get; set; } = null!;
    public string Status { get; set; } =  null!;
    public List<PlanDayDto> Days  { get; set; } = null!;
    public decimal TotalFees { get; set; }
    public DateTime StartDate { get; set; }
    public int Period { get; set; }
}