using Drosy.Domain.Shared.ResultPattern;

namespace Drosy.Domain.Interfaces.Uow
{
    public interface IUnitOfWork
    {
        Task<bool> SaveChangesAsync(CancellationToken  cancellationToken);
    }
}