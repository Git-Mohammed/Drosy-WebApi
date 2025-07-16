using Drosy.Domain.Shared.ResultPattern;
using Drosy.Domain.Shared.ResultPattern.ErrorComponents;
using Microsoft.AspNetCore.Mvc;

namespace Drosy.Api.Commons.Responses
{
    /// <summary>
    /// Provides standardized methods for generating HTTP API responses in a consistent structure.
    /// </summary>
    public static class ResponseHandler
    {
        /// <summary>
        /// Returns a 200 OK response with a success message and data.
        /// </summary>
        /// <typeparam name="T">Type of the returned data.</typeparam>
        /// <param name="data">The data to return in the response.</param>
        /// <param name="message">Optional success message.</param>
        /// <returns>An <see cref="IActionResult"/> containing a successful response.</returns>
        public static IActionResult SuccessResponse<T>(T data, string message = "Request successful")
        {
            return new ObjectResult(ApiResponse<T>.Success(data, message))
            {
                StatusCode = StatusCodes.Status200OK
            };
        }

        /// <summary>
        /// Returns a 200 OK response with a success message and no data.
        /// </summary>
        /// <param name="message">Optional success message.</param>
        /// <returns>An <see cref="IActionResult"/> indicating a successful operation.</returns>
        public static IActionResult SuccessResponse(string message = "Operation completed successfully")
        {
            return new ObjectResult(ApiResponse<object>.Success(null, message))
            {
                StatusCode = StatusCodes.Status200OK
            };
        }

        /// <summary>
        /// Returns a 400 Bad Request response with property-specific errors.
        /// </summary>
        /// <param name="property">The property that caused the error.</param>
        /// <param name="message">The main error message.</param>
        /// <param name="details">Optional additional error details.</param>
        /// <returns>An <see cref="IActionResult"/> with error information.</returns>
        public static IActionResult BadRequestResponse(string property, string message, params string[] details)
        {
            var errors = BuildErrors(property, message, details);
            return new ObjectResult(ApiResponse<object>.Failure(errors, message))
            {
                StatusCode = StatusCodes.Status400BadRequest
            };
        }
        
        /// <summary>
        /// Returns a 400 Bad Request response with a list of predefined <see cref="ApiError"/>s.
        /// </summary>
        /// <param name="errors">A list of API errors.</param>
        /// <param name="message">Error summary message.</param>
        /// <returns>An <see cref="IActionResult"/> representing a bad request with error details.</returns>
        public static IActionResult BadRequestResponse(List<ApiError> errors, string message)
        {
            return new ObjectResult(ApiResponse<object>.Failure(errors, message))
            {
                StatusCode = StatusCodes.Status400BadRequest
            };
        }

        /// <summary>
        /// Returns a 404 Not Found response when a resource is missing.
        /// </summary>
        /// <param name="property">The property or resource name causing the error.</param>
        /// <param name="message">Main error message.</param>
        /// <param name="details">Optional detailed error messages.</param>
        /// <returns>An <see cref="IActionResult"/> representing a not found error.</returns>
        public static IActionResult NotFoundResponse(string property, string message, params string[] details)
        {
            var errors = BuildErrors(property, message, details);
            return new ObjectResult(ApiResponse<object>.Failure(errors, message))
            {
                StatusCode = StatusCodes.Status404NotFound
            };
        }

        /// <summary>
        /// Returns a 401 Unauthorized response for unauthorized access attempts.
        /// </summary>
        /// <param name="property">The related property or area of failure.</param>
        /// <param name="message">Main error message.</param>
        /// <param name="details">Optional detailed error messages.</param>
        /// <returns>An <see cref="IActionResult"/> representing unauthorized access.</returns>
        public static IActionResult UnauthorizedResponse(string property, string message, params string[] details)
        {
            var errors = BuildErrors(property, message, details);
            return new ObjectResult(ApiResponse<object>.Failure(errors, message))
            {
                StatusCode = StatusCodes.Status401Unauthorized
            };
        }

        /// <summary>
        /// Returns a 204 No Content response when no result is available but the operation succeeded.
        /// </summary>
        /// <param name="message">Optional message describing the result.</param>
        /// <returns>An <see cref="IActionResult"/> indicating successful operation with no content.</returns>
        public static IActionResult NoContentResponse(string message = "No content available")
        {
            return new ObjectResult(ApiResponse<object>.Success(null, message))
            {
                StatusCode = StatusCodes.Status204NoContent
            };
        }

        /// <summary>
        /// Returns a 201 Created response with route information and created data.
        /// </summary>
        /// <typeparam name="T">Type of the created resource.</typeparam>
        /// <param name="routeName">The name of the route to retrieve the resource.</param>
        /// <param name="routeValues">The values for the route parameters.</param>
        /// <param name="data">The data of the newly created resource.</param>
        /// <param name="message">Optional success message.</param>
        /// <returns>An <see cref="IActionResult"/> with status code 201.</returns>
        public static IActionResult CreatedResponse<T>(string routeName, object routeValues, T data, string message = "Resource created successfully")
        {
            return new CreatedAtRouteResult(routeName, routeValues, ApiResponse<T>.Success(data, message));
        }

        /// <summary>
        /// Returns a response with a custom status code and optional error details.
        /// </summary>
        /// <param name="statusCode">HTTP status code.</param>
        /// <param name="property">The related property or context.</param>
        /// <param name="message">Main message describing the result or error.</param>
        /// <param name="details">Optional additional messages.</param>
        /// <returns>An <see cref="IActionResult"/> with custom status and content.</returns>
        public static IActionResult StatusCodeResponse(int statusCode, string property, string message, params string[] details)
        {
            bool isSuccess = statusCode is >= 200 and < 300;
            var errors = isSuccess ? null : BuildErrors(property, message, details);

            return new ObjectResult(new ApiResponse<object>
            {
                IsSuccess = isSuccess,
                Message = message,
                Data = null,
                Errors = errors
            })
            { StatusCode = statusCode };
        }
        
        /// <summary>
        /// Generates a structured error response based on a domain result object.
        /// Maps domain-specific error codes to HTTP status codes.
        /// </summary>
        /// <param name="result">The result object containing the error.</param>
        /// <param name="operation">The name of the failed operation (e.g., "Create", "Delete").</param>
        /// <param name="entity">Optional entity name related to the operation.</param>
        /// <returns>An appropriate <see cref="IActionResult"/> with mapped status and message.</returns>
        public static IActionResult HandleFailure(Result result, string operation, string? entity = null)
        {
            var errorCode = result.Error.Code;
            var errorMessage = result.Error.Message;
            var error = new ApiError(operation, errorMessage);

            var status = errorCode switch
            {
                nameof(Error.NotFound) => 404,
                nameof(Error.Failure) => 500,
                nameof(Error.NullValue) => 400,
                nameof(Error.Invalid) => 422,
                nameof(Error.Unauthorized) => 401,
                nameof(Error.Conflict) => 409,
                nameof(Error.OperationCancelled) => 202,
                nameof(Error.BusinessRule) => 422,
                _ => 400
            };

            var responseMessage = errorCode switch
            {
                nameof(Error.NotFound) => $"{entity?.ToLower()} not found.",
                nameof(Error.Failure) => $"Failed to {operation.ToLower()}.",
                nameof(Error.NullValue) => "Invalid data.",
                nameof(Error.Invalid) => "Validation failed.",
                nameof(Error.Unauthorized) => "Unauthorized access.",
                nameof(Error.Conflict) => "Conflict detected.",
                nameof(Error.OperationCancelled) => "Operation cancelled.",
                nameof(Error.BusinessRule) => "Business Rule Violation.",
                _ => "Unknown error occurred."
            };

            return StatusCodeResponse(status, error.Property, responseMessage, errorMessage);
        }
        
        /// <summary>
        /// Handles unexpected exceptions by returning a standardized 500 Internal Server Error response.
        /// </summary>
        /// <param name="ex">The exception that occurred.</param>
        /// <returns>An <see cref="IActionResult"/> representing a server error.</returns>
        public static IActionResult HandleException(Exception ex)
        {
            var error = new ApiError("Exception", ErrorMessagesRepository.GetMessage(nameof(Error.Failure), Error.CurrentLanguage));
            return StatusCodeResponse(500, "Exception", error.Message);
        }
        
        /// <summary>
        /// Builds a list of <see cref="ApiError"/>s from a main message and optional detailed messages.
        /// </summary>
        /// <param name="property">The property or field related to the error.</param>
        /// <param name="message">The main error message.</param>
        /// <param name="details">Optional array of detailed messages.</param>
        /// <returns>A list of <see cref="ApiError"/>s.</returns>
        private static List<ApiError> BuildErrors(string property, string message, params string[] details)
        {
            var errors = new List<ApiError> { new(property, message) };
            if (details != null && details.Length > 0)
            {
                errors.AddRange(details.Select(d => new ApiError(property, d)));
            }
            return errors;
        }
    }
}
