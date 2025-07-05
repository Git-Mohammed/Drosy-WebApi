using Drosy.Domain.Shared.ResultPattern;

namespace Drosy.Application.Interfaces.Common
{
    public interface IUnitOfWork : IDisposable
    {
        Task<Result> SaveChangesAsync();

        Task<Result> StartTransactionAsync();

        Task<Result> CommitAsync();

        Task<Result> RollbackAsync();
    }
}