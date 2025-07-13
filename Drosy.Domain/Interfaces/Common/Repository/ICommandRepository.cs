namespace Drosy.Domain.Interfaces.Common.Repository
{
    public interface ICommandRepository<in TEntity> where TEntity : class
    {
        #region Add
        Task AddAsync(TEntity entity, CancellationToken cancellationToken);
        Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken);
        #endregion

        #region Update
        Task UpdateAsync(TEntity entity, CancellationToken cancellationToken);
        Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken);
        #endregion

        #region Delete
        Task DeleteAsync(TEntity entity,  CancellationToken cancellationToken);
        Task DeleteRangeAsync(IEnumerable<TEntity> entities,  CancellationToken cancellationToken);
        #endregion
    }
}