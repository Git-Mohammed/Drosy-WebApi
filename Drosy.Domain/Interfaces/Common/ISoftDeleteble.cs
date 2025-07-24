namespace Drosy.Domain.Interfaces.Common
{
    public interface ISoftDeleteble
    {
        bool IsDeleted { get; set; }
    }
}
