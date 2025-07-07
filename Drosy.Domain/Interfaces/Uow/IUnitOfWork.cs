using Drosy.Domain.Shared.ResultPattern;

namespace Drosy.Domain.Interfaces.Uow
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken  cancellationToken);
    }
}