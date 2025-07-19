namespace Drosy.Domain.Shared.ErrorComponents.EFCore;

/// <summary>
/// Defines EF Core specific error codes.
/// </summary>
public static class EfCoreErrors
{
    public static AppError NoChanges => new(EfCoreErrorCodes.NoChanges);
    public static AppError CanNotSaveChanges => new(EfCoreErrorCodes.CanNotSaveChanges);
    public static AppError FailedTransaction => new(EfCoreErrorCodes.FailedTransaction);
    // Add any other specific EF Core related errors here
    public static AppError ConcurrencyConflict => new(EfCoreErrorCodes.ConcurrencyConflict);
    public static AppError ConstraintViolation => new(EfCoreErrorCodes.ConstraintViolation);
}