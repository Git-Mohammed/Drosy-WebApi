using Drosy.Domain.Enums;

namespace Drosy.Domain.Entities;

public class PlanDay
{
    public int Id { get; set; }
    public int PlanId { get; set; }
    public Days Day { get; set; }
    public TimeSpan StartSession { get; set; }
    public TimeSpan EndSession { get; set; }

    #region Navigation Properties
    public Plan Plan { get; set; } = new();
    #endregion
}