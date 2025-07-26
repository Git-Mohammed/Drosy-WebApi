namespace Drosy.Domain.Entities
{
    public class Session : BaseEntity<int>
    {
        public int PlanId { get; set; }
        public string Title { get; set; } = null!;
        public DateTime ExcepectedDate { get; set; } 
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? Notes { get; set; }

        #region Navigations
        public Plan Plan { get; set; } = null!;
        public List<Attendence> Attendences { get; set; } = new();
        #endregion
    }
}
