using Drosy.Domain.Enums;
using Drosy.Domain.Interfaces.Common;

namespace Drosy.Domain.Entities;

public class Payment : BaseEntity<int>, ICreateAt
{
    public int PlanId { get; set; }
    public int StudentId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; } = PaymentMethod.Cash;
    public string? Notes { get; set; } 
    public DateTime CreatedAt { get; set; }
        
    #region Navigation properties
    public Plan? Plan { get; set; }
    public Student? Student { get; set; }
    #endregion
}