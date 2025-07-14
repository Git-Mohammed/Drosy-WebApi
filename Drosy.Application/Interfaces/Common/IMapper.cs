namespace Drosy.Application.Interfaces.Common
{
    /// <summary>
    /// Defines mapping functionality between different object types.
    /// </summary>
    public interface IMapper
    {
        /// <summary>
        /// Creates a new instance of <typeparamref name="TDestination"/> 
        /// and maps properties from the specified <typeparamref name="TSource"/> object.
        /// </summary>
        /// <typeparam name="TSource">The source type from which data is mapped.</typeparam>
        /// <typeparam name="TDestination">The destination type to which data is mapped.</typeparam>
        /// <param name="source">The source object containing data.</param>
        /// <returns>A new instance of <typeparamref name="TDestination"/> with mapped properties.</returns>
        TDestination Map<TSource, TDestination>(TSource source);

        /// <summary>
        /// Maps properties from the specified <typeparamref name="TSource"/> object 
        /// into an existing <typeparamref name="TDestination"/> instance.
        /// </summary>
        /// <typeparam name="TSource">The source type from which data is mapped.</typeparam>
        /// <typeparam name="TDestination">The destination type to which data is mapped.</typeparam>
        /// <param name="source">The source object containing data.</param>
        /// <param name="destination">The existing destination object to be updated with mapped properties.</param>
        void Map<TSource, TDestination>(TSource source, TDestination destination);
    }
}
