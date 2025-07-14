using Drosy.Domain.Interfaces.Repository;

namespace Drosy.Domain.Interfaces.Common.Repository
{
    public interface IRepository<TEntity> : ICommandRepository<TEntity>, IQueryRepository<TEntity> where TEntity : class
    {

    }
}