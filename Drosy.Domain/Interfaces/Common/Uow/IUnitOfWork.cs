namespace Drosy.Domain.Interfaces.Common.Uow
{
    /// <summary>
    /// Represents the Unit of Work pattern, which is used to group one or more operations 
    /// into a single transaction that can be committed or rolled back as a whole.
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// Persists all changes made within the current unit of work to the data store.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result indicates whether the changes were successfully saved.
        /// </returns>
        Task<bool> SaveChangesAsync(CancellationToken cancellationToken);
    }
}