using Drosy.Domain.Enums;

namespace Drosy.Domain.Entities;

public class Plan : BaseEntity<int>
{
    public PlanTypes Type { get; set; }
    public PlanStatus Status { get; set; }
    public decimal TotalFees { get; set; }
    
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    #region Navigation Properties
    public List<PlanStudent> Students { get; set; } = new();
    public List<Session> Sessions { get; set; } = new();
    public List<Payment> Payments { get; set; } = new();
    public List<PlanDay> PlanDays { get; set; } = new();
    #endregion
}