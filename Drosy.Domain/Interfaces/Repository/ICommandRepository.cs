namespace Drosy.Domain.Interfaces.Repository
{
    public interface ICommandRepository<in TEntity> where TEntity : class
    {
        #region Add
        Task AddAsync(TEntity entity);
        Task AddRangeAsync(IEnumerable<TEntity> entities);
        void Add(TEntity entity);
        void AddRange(IEnumerable<TEntity> entities);
        #endregion

        #region Update
        Task UpdateAsync(TEntity entity);
        void Update(TEntity entity);
        Task UpdateRangeAsync(IEnumerable<TEntity> entities);
        void UpdateRange(IEnumerable<TEntity> entities);
        #endregion

        #region Delete
        Task DeleteAsync(TEntity entity);
        Task DeleteRangeAsync(IEnumerable<TEntity> entities);
        void Delete(TEntity entity);
        void DeleteRange(IEnumerable<TEntity> entities);
        #endregion

        #region SaveChanges
        Task<int> SaveChangesAsync();
        int SaveChanges();
        #endregion
    }
}