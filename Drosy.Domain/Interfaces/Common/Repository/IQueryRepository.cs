namespace Drosy.Domain.Interfaces.Common.Repository
{
    public interface IQueryRepository<TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken);
    }

}
