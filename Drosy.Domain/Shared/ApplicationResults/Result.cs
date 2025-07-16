using System.Diagnostics.CodeAnalysis;
using Drosy.Domain.Shared.ErrorComponents;
using Drosy.Domain.Shared.ResultPattern.ErrorComponents.Common;

namespace Drosy.Domain.Shared.ApplicationResults
{
    /// <summary>
    /// Represents the outcome of an operation, indicating success or failure.
    /// Optionally, for a failure result, an exception can be attached.
    /// </summary>
    public class Result
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Result"/> class.
        /// Throws an <see cref="InvalidOperationException"/> if the combination of parameters is not valid.
        /// </summary>
        /// <param name="isSuccess">True if the result represents success; otherwise, false.</param>
        /// <param name="error">
        /// The <see cref="Error"/>.
        /// Must be <see cref="AppError.None"/> if <paramref name="isSuccess"/> is true,
        /// and must not be <see cref="AppError.None"/> if <paramref name="isSuccess"/> is false.
        /// </param>
        /// <param name="exception">Optional exception associated with a failed operation.</param>
        protected Result(bool isSuccess, AppError error, Exception? exception = null)
        {
            switch (isSuccess)
            {
                case true when error != AppError.None:
                    throw new InvalidOperationException("A successful result cannot have an error.");
                case false when error == AppError.None:
                    throw new InvalidOperationException("A failure result must have a valid error.");
                default:
                    IsSuccess = isSuccess;
                    Error = error;
                    Exception = exception;
                    break;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the operation succeeded.
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// Gets a value indicating whether the operation failed.
        /// </summary>
        public bool IsFailure => !IsSuccess;

        /// <summary>
        /// Gets the error associated with a failure. For a successful result, this is <see cref="AppError.None"/>.
        /// </summary>
        public AppError Error { get; }

        /// <summary>
        /// Gets the exception associated with a failure, if provided.
        /// </summary>
        public Exception? Exception { get; }

        /// <summary>
        /// Creates a successful result.
        /// </summary>
        public static Result Success() => new(true, AppError.None);

        /// <summary>
        /// Creates a failure result with a given error.
        /// </summary>
        public static Result Failure(AppError error) => new(false, error);

        /// <summary>
        /// Creates a failure result with a given error and exception.
        /// </summary>
        public static Result Failure(AppError error, Exception exception) => new(false, error, exception);

        /// <summary>
        /// Creates a successful result containing a value.
        /// </summary>
        public static Result<T> Success<T>(T value) => new(value, true, AppError.None);

        /// <summary>
        /// Creates a failure result containing a value with a given error.
        /// </summary>
        public static Result<T> Failure<T>(AppError error) => new(default, false, error);

        /// <summary>
        /// Creates a failure result containing a value with a given error and exception.
        /// </summary>
        public static Result<T> Failure<T>(AppError error, Exception exception)
            => new(default, false, error, exception);

        /// <summary>
        /// Creates a result based on whether the provided value is null.
        /// If non-null, a successful result is returned; otherwise, a failure result is returned with <see cref="AppError.NullValue"/>.
        /// </summary>
        public static Result<T> Create<T>(T? value) =>
            value is not null ? Success(value) : Failure<T>(CommonErrors.NullValue);
    }

    /// <summary>
    /// Represents the outcome of an operation that returns a value of type <typeparamref name="T"/>.
    /// </summary>
    public class Result<T> : Result
    {
        private readonly T? _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="Result{T}"/> class.
        /// </summary>
        /// <param name="value">The value, if the operation was successful.</param>
        /// <param name="isSuccess">Indicates whether the result represents a success.</param>
        /// <param name="error">
        /// The error associated with the result.
        /// Must be <see cref="AppError.None"/> if <paramref name="isSuccess"/> is true,
        /// and must not be <see cref="AppError.None"/> if <paramref name="isSuccess"/> is false.
        /// </param>
        protected internal Result(T? value, bool isSuccess, AppError error)
            : base(isSuccess, error)
        {
            _value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Result{T}"/> class with an exception.
        /// </summary>
        /// <param name="value">The value, if any. Usually default in a failure case.</param>
        /// <param name="isSuccess">Indicates whether the result represents a success.</param>
        /// <param name="error">
        /// The error associated with the result.
        /// Must be <see cref="AppError.None"/> if <paramref name="isSuccess"/> is true,
        /// and must not be <see cref="AppError.None"/> if <paramref name="isSuccess"/> is false.
        /// </param>
        /// <param name="exception">The exception to associate with a failure result.</param>
        protected internal Result(T? value, bool isSuccess, AppError error, Exception? exception)
            : base(isSuccess, error, exception)
        {
            _value = value;
        }

        /// <summary>
        /// Gets the value of the successful result.
        /// Throws an <see cref="InvalidOperationException"/> if the result is a failure.
        /// </summary>
        [NotNull]
        public T Value => _value ?? throw new InvalidOperationException("Result has no value");

        /// <summary>
        /// Implicit conversion from a value of type <typeparamref name="T"/> to <see cref="Result{T}"/>.
        /// </summary>
        public static implicit operator Result<T>(T? value) => Create(value);
    }
}