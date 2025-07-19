namespace Drosy.Domain.Interfaces.Common.Repository
{
    /// <summary>
    /// Represents a generic repository interface that combines 
    /// read and write operations for a specific entity type.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public interface IRepository<TEntity> : ICommandRepository<TEntity>, IReadOnlyRepository<TEntity> where TEntity : class
    {
        // This interface serves as a unified abstraction for both query and command operations.
        // No additional members are declared here.
    }
}