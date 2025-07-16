namespace Drosy.Domain.Shared.DataDTOs
{
    /// <summary>
    /// Represents a paginated result set that inherits from <see cref="DataResult{T}"/>.
    /// Includes pagination metadata such as page number, page size, and navigation flags.
    /// </summary>
    /// <typeparam name="T">The type of the data items returned.</typeparam>
    public class PaginatedResult<T> : DataResult<T>
    {
        /// <summary>
        /// Gets or sets the current page number (1-based index).
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// Gets or sets the number of items per page.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Gets a value indicating whether there is a previous page.
        /// </summary>
        public bool HasPreviousPage => PageNumber > 1;

        /// <summary>
        /// Gets a value indicating whether there is a next page.
        /// </summary>
        public bool HasNextPage => PageNumber < TotalPages;

        /// <summary>
        /// Gets the total number of pages, calculated from <c>TotalRecordsCount</c> and <c>PageSize</c>.
        /// </summary>
        public int TotalPages => TotalRecordsCount > 0 && PageSize > 0
            ? (int)Math.Ceiling((double)TotalRecordsCount / PageSize)
            : 0;
    }
}
