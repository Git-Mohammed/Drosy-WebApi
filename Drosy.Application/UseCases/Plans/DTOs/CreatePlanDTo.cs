
namespace Drosy.Application.UseCases.Plans.DTOs;

public class CreatePlanDTo
{
    public string Type { get; set; } = null!;
    public string Status { get; set; } =  null!;
    public List<string> DaysOfWeek  { get; set; } = null!;
    public decimal TotalFees { get; set; }
    public DateTime StartDate { get; set; }
    public int Period { get; set; }
    public TimeSpan StartSession {get; set;}
    public TimeSpan EndSession { get; set; }
}