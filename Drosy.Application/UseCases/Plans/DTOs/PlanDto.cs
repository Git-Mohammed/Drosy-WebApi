namespace Drosy.Application.UseCases.Plans.DTOs;

public class PlanDto
{
    public int Id { get; set; }
    public string Type { get; set; } = null!;
    public string Status { get; set; } =  null!;
    public List<PlanDayDto> Days  { get; set; } = new List<PlanDayDto>();
    public decimal TotalFees { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int SessionPeriod { get; set; }
    public int Period { get; set; }
}