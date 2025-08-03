using Drosy.Domain.Interfaces.Common;

namespace Drosy.Domain.Entities
{
    public class Subject : BaseEntity<int>, ISoftDeleteble
    {
        public string Name { get; set; } = null!;
        public bool IsDeleted { get ; set ; }
        public int? DeletedBy { get; set; }
        public DateTime? DeletedAt { get; set ; }
    }
}