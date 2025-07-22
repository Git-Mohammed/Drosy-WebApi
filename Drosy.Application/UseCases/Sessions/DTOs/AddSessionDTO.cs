namespace Drosy.Application.UseCases.Sessions.DTOs;

public class AddSessionDTO
{
    public int PlanId { get; set; }
    public string Title { get; set; } = null!;
    public DateTime ExcepectedDate { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

}