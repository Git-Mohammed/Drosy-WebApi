namespace Drosy.Api.Commons.Responses
{
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = String.Empty;
        public T? Data { get; set; }
        public List<ApiError>? Errors { get; set; }

        public static ApiResponse<T> Success(T data, string message = "")
            => new ApiResponse<T> { IsSuccess = true, Message = message, Data = data };

        public static ApiResponse<T> Failure(List<ApiError> errors, string message = "")
        => new ApiResponse<T> { IsSuccess = false, Message = message, Errors = errors };
    }

    public record ApiError(string Property, string Message);
}