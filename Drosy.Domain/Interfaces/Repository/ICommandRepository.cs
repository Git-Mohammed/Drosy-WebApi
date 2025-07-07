namespace Drosy.Domain.Interfaces.Repository
{
    public interface ICommandRepository<in TEntity> where TEntity : class
    {
        #region Add
        Task AddAsync(TEntity entity);
        Task AddRangeAsync(IEnumerable<TEntity> entities);
        #endregion

        #region Update
        Task UpdateAsync(TEntity entity);
        Task UpdateRangeAsync(IEnumerable<TEntity> entities);
        #endregion

        #region Delete
        Task DeleteAsync(TEntity entity);
        Task DeleteRangeAsync(IEnumerable<TEntity> entities);
        #endregion
    }
}