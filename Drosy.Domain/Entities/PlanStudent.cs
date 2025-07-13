using Drosy.Domain.Interfaces.Common;

namespace Drosy.Domain.Entities
{
    public class PlanStudent : ICreateAt
    {
        public int PlanId { get; set; }
        public int StudentId { get; set; }
        public string Fee { get; set; } = null!;
        public string Notes { get; set; } = null!;
        public DateTime CreatedAt { get; set; } 

        #region Nav Properties
        public Student Student { get; set; } = null!;
        public Plan Plan { get; set; } = null!;
        #endregion
    }

}


