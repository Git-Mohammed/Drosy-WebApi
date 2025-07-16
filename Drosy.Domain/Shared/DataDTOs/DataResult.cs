namespace Drosy.Domain.Shared.DataDTOs
{
    /// <summary>
    /// Represents a standard structure for returning data collections, 
    /// including filtered and total record counts.
    /// </summary>
    /// <typeparam name="T">The type of the data items.</typeparam>
    public class DataResult<T>
    {
        /// <summary>
        /// Gets or sets the collection of data records, possibly filtered.
        /// </summary>
        public IEnumerable<T> Data { get; set; } = null!;

        /// <summary>
        /// Gets the count of records in the <see cref="Data"/> collection (i.e., filtered records).
        /// </summary>
        public int DataRecordsCount => Data.Count();

        /// <summary>
        /// Gets or sets the total number of records in the data source (i.e., before applying any filters).
        /// </summary>
        public int TotalRecordsCount { get; set; }
    }
}
