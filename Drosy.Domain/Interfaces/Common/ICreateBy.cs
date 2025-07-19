namespace Drosy.Domain.Interfaces.Common
{
    public interface ICreateBy<T>
    {
        T CreatedBy { get; set; }
    }
}
