namespace Drosy.Application.UseCases.Dashboard.DTOs;
public class DashboardDTO
{
    public int TotalScheduledSessions { get; set; }
    public DashboardStatsViewDTO DashboardStats { get; set; } = null!;
    public List<DashboardSessionDTO> TodayScheduledSessions { get; set; } = new();
}
