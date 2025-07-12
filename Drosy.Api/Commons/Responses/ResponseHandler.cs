using Drosy.Domain.Shared.ResultPattern;
using Drosy.Domain.Shared.ResultPattern.ErrorComponents;
using Microsoft.AspNetCore.Mvc;

namespace Drosy.Api.Commons.Responses
{
    public static class ResponseHandler
    {
        public static IActionResult SuccessResponse<T>(T data, string message = "Request successful")
        {
            return new ObjectResult(ApiResponse<T>.Success(data, message))
            {
                StatusCode = StatusCodes.Status200OK
            };
        }

        public static IActionResult SuccessResponse(string message = "Operation completed successfully")
        {
            return new ObjectResult(ApiResponse<object>.Success(null, message))
            {
                StatusCode = StatusCodes.Status200OK
            };
        }

        public static IActionResult BadRequestResponse(string property, string message, params string[] details)
        {
            var errors = BuildErrors(property, message, details);
            return new ObjectResult(ApiResponse<object>.Failure(errors, message))
            {
                StatusCode = StatusCodes.Status400BadRequest
            };
        }
        
        public static IActionResult BadRequestResponse(List<ApiError> errors, string message)
        {
            return new ObjectResult(ApiResponse<object>.Failure(errors, message))
            {
                StatusCode = StatusCodes.Status400BadRequest
            };
        }


        public static IActionResult NotFoundResponse(string property, string message, params string[] details)
        {
            var errors = BuildErrors(property, message, details);
            return new ObjectResult(ApiResponse<object>.Failure(errors, message))
            {
                StatusCode = StatusCodes.Status404NotFound
            };
        }

        public static IActionResult UnauthorizedResponse(string property, string message, params string[] details)
        {
            var errors = BuildErrors(property, message, details);
            return new ObjectResult(ApiResponse<object>.Failure(errors, message))
            {
                StatusCode = StatusCodes.Status401Unauthorized
            };
        }

        public static IActionResult NoContentResponse(string message = "No content available")
        {
            return new ObjectResult(ApiResponse<object>.Success(null, message))
            {
                StatusCode = StatusCodes.Status204NoContent
            };
        }

        public static IActionResult CreatedResponse<T>(string routeName, object routeValues, T data, string message = "Resource created successfully")
        {
            return new CreatedAtRouteResult(routeName, routeValues, ApiResponse<T>.Success(data, message));
        }

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
                _ => "Unknown error occurred."
            };

            return StatusCodeResponse(status, error.Property, responseMessage, errorMessage);
        }
        public static IActionResult HandleException(Exception ex)
        {
            var error = new ApiError("Exception", ErrorMessagesRepository.GetMessage(nameof(Error.Failure), Error.CurrentLanguage));
            return StatusCodeResponse(500, "Exception", error.Message);
        }
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
