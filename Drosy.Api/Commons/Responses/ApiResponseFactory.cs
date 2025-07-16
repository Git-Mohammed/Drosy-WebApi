using Drosy.Domain.Shared.ApplicationResults;
using Drosy.Domain.Shared.ErrorComponents;
using Drosy.Domain.Shared.ErrorComponents.Common;
using Microsoft.AspNetCore.Mvc;

namespace Drosy.Api.Commons.Responses
{
    /// <summary>
    /// Provides standardized methods for generating HTTP API responses in a consistent structure.
    /// </summary>
    public static class ApiResponseFactory
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
        /// Creates a standardized response with a custom HTTP status code and optional error details.
        /// </summary>
        /// <param name="statusCode">HTTP status code.</param>
        /// <param name="property">The related property or context.</param>
        /// <param name="message">Main message describing the result or error.</param>
        /// <param name="details">Optional additional messages.</param>
        /// <returns>An <see cref="IActionResult"/> with custom status and content.</returns>
        public static IActionResult CreateStatusResponse(int statusCode, string property, string message, params string[] details)
        {
            bool isSuccess = statusCode is >= 200 and < 300;

            var response = new ApiResponse<object>
            {
                IsSuccess = isSuccess,
                Message = message,
                Data = null,
                Errors = isSuccess ? null : BuildErrors(property, message, details)
            };

            return new ObjectResult(response) { StatusCode = statusCode };
        }

        /// <summary>
        /// Converts a failed <see cref="Result"/> into a standardized <see cref="IActionResult"/>.
        /// Maps the domain error code to an appropriate HTTP status code and returns a localized error response.
        /// </summary>
        /// <param name="result">The domain result object representing a failed operation.</param>
        /// <param name="operation">The name of the operation that failed (e.g., "Create", "Update").</param>
        /// <param name="entity">Optional name of the entity involved in the operation.</param>
        /// <returns>An <see cref="IActionResult"/> containing the mapped status code and localized error message.</returns>
        public static IActionResult FromFailure(Result result, string operation, string? entity = null)
        {
            string errorCode = result.Error.Code;
            string errorMessage = result.Error.Message;

            var statusCode = MapStatusCode(errorCode);
            var localizedMessage = LocalizedErrorMessageProvider.GetMessage(errorCode, AppError.CurrentLanguage);

            return CreateStatusResponse(statusCode, operation, localizedMessage, errorMessage);
        }

        /// <summary>
        /// Converts an unhandled exception into a standardized 500 Internal Server Error response.
        /// </summary>
        public static IActionResult FromException(Exception ex)
        {
            string message = ErrorMessageResourceRepository.GetMessage(CommonErrorCodes.Unexpected, AppError.CurrentLanguage);
            return CreateStatusResponse(StatusCodes.Status500InternalServerError, "Exception", message);
        }

        /// <summary>
        /// Maps domain error codes to HTTP status codes.
        /// </summary>
        private static int MapStatusCode(string errorCode) => errorCode switch
        {
            CommonErrorCodes.NotFound => StatusCodes.Status404NotFound,
            CommonErrorCodes.Failure => StatusCodes.Status500InternalServerError,
            CommonErrorCodes.NullValue => StatusCodes.Status400BadRequest,
            CommonErrorCodes.Invalid => StatusCodes.Status422UnprocessableEntity,
            CommonErrorCodes.Unauthorized => StatusCodes.Status401Unauthorized,
            CommonErrorCodes.Conflict => StatusCodes.Status409Conflict,
            CommonErrorCodes.OperationCancelled => StatusCodes.Status202Accepted,
            CommonErrorCodes.BusinessRule => StatusCodes.Status422UnprocessableEntity,
            _ => StatusCodes.Status400BadRequest
        };


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
