namespace Drosy.Application.UseCases.Plans.DTOs;

public class PlanDto
{
    public int Id { get; set; }
    public string Type { get; set; } = null!;
    public string Status { get; set; } =  null!;
    public List<string> DaysOfWeek  { get; set; } = null!;
    public decimal TotalFees { get; set; }
    public DateTime StartDate { get; set; }
    public int Period { get; set; }
    public DateTime EndDate { get; set; }
    
    public TimeSpan StartSession {get; set;}
    public TimeSpan EndSession { get; set; }
    public float SessionPeriod { get; set; }
}