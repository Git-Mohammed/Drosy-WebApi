namespace Drosy.Domain.Interfaces.Repository
{
    public interface IRepository<TEntity> : ICommandRepository<TEntity>, IQueryRepository<TEntity> where TEntity : class
    {

    }
}