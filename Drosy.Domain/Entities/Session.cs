using Drosy.Domain.Enums;
using Drosy.Domain.Interfaces.Common;

namespace Drosy.Domain.Entities
{
    public class Session : BaseEntity<int>,ICreateAt
    {
        public int PlanId { get; set; }
        public string Title { get; set; } = null!;
        public SessionStatus Status { get; set; }   
        public DateTime CreatedAt { get; set; } 
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? Notes { get; set; }

        #region Navigations
        public Plan Plan { get; set; } = null!;
        public List<Attendence> Attendences { get; set; } = new();
        #endregion
    }
}
