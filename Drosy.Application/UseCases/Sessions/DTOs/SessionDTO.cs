using Drosy.Application.UseCases.Plans.DTOs;

namespace Drosy.Application.UseCases.Sessions.DTOs;

public class SessionDTO
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public DateTime ExcepectedDate { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    #region Navigations
    public PlanDto Plan { get; set; } = null!;
    #endregion
}
