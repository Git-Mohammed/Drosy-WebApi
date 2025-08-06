namespace Drosy.Domain.Interfaces.Common.Repository
{
    /// <summary>
    /// Defines write operations (Add, Update, Delete) for an entity.
    /// This is a part of the Command side in the Repository Pattern.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity being managed.</typeparam>
    public interface ICommandRepository<in TEntity> where TEntity : class
    {
        #region Add

        /// <summary>
        /// Asynchronously adds a new entity to the data store.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task AddAsync(TEntity entity, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously adds multiple entities to the data store.
        /// </summary>
        /// <param name="entities">A collection of entities to add.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken);

        #endregion

        #region Update

        /// <summary>
        /// Asynchronously updates an existing entity in the data store.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task UpdateAsync(TEntity entity, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously updates multiple entities in the data store.
        /// </summary>
        /// <param name="entities">A collection of entities to update.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken);

        #endregion

        #region Delete

        /// <summary>
        /// Asynchronously deletes an entity from the data store.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task DeleteAsync(TEntity entity, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously deletes multiple entities from the data store.
        /// </summary>
        /// <param name="entities">A collection of entities to delete.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken);


        public Task SoftDeleteAsync(TEntity entity, int deletedBy, CancellationToken ct);
        #endregion
    }
}
