namespace Drosy.Domain.Interfaces.Repository
{
    public interface IQueryRepository<TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAllAsync();
        IEnumerable<TEntity> GetAll();
    }

}
