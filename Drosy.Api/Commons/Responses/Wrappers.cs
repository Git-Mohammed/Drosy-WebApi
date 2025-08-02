using Drosy.Domain.Shared.ApplicationResults;
using Microsoft.AspNetCore.Mvc;

namespace Drosy.Api.Commons.Responses
{
    /// <summary>
    /// Provides utility methods to standardize API response formatting and error handling.
    /// </summary>
    public static class Wrappers
    {
        /// <summary>
        /// Executes a data-fetching operation wrapped in a standardized API response structure.
        /// </summary>
        /// <typeparam name="T">The type of the result value returned by the operation.</typeparam>
        /// <param name="op">The asynchronous operation that returns a <see cref="Result{T}"/>.</param>
        /// <param name="successMsg">A message to include in the response if the operation succeeds.</param>
        /// <param name="method">The name of the method invoking this wrapper (for logging/diagnostics).</param>
        /// <param name="entity">The name of the domain entity involved in the operation.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> representing the standardized API response:
        /// - 200 OK with data if successful
        /// - Error response if failure or exception occurs
        /// </returns>
        public static async Task<IActionResult> WrapListFilter<T>(
            Func<Task<Result<T>>> op,
            string successMsg,
            string method,
            string entity)
        {
            try
            {
                var result = await op();
                if (result.IsFailure)
                    return ApiResponseFactory.FromFailure(result, method, entity);

                return ApiResponseFactory.SuccessResponse(result.Value, successMsg);
            }
            catch (Exception ex)
            {
                return ApiResponseFactory.FromException(ex);
            }
        }
    }
}
