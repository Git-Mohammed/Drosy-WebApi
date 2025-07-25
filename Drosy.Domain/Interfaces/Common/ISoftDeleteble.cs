namespace Drosy.Domain.Interfaces.Common
{
    public interface ISoftDeleteble
    {
        bool IsDeleted { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
