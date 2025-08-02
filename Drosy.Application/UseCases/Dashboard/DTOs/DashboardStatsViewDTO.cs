namespace Drosy.Application.UseCases.Dashboard.DTOs;

public class DashboardStatsViewDTO
{
    public int TotalRegisteredStudents { get; set; }
    public decimal TotalAmountDue { get; set; }
    public decimal TotalAmountCollected { get; set; }
}
