namespace Drosy.Domain.Interfaces.Common.Repository
{
    /// <summary>
    /// Defines read-only data access operations for a specific entity type.
    /// This is part of the Query side in the Repository Pattern.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity to retrieve.</typeparam>
    public interface IReadOnlyRepository<TEntity> where TEntity : class
    {
        /// <summary>
        /// Asynchronously retrieves all entities of the specified type from the data store.
        /// </summary>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.  
        /// The task result contains a collection of all entities.
        /// </returns>
        Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken);
    }
}