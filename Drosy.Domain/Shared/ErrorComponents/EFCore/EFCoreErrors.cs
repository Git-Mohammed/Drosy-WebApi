namespace Drosy.Domain.Shared.ErrorComponents.EFCoreErrors;

using Drosy.Domain.Shared.ErrorComponents;

/// <summary>
/// Defines EF Core specific error codes.
/// </summary>
public static class EFCoreErrors
{
    public static AppError NoChanges => new(EFCoreErrorCodes.NoChanges);
    public static AppError CanNotSaveChanges => new(EFCoreErrorCodes.CanNotSaveChanges);
    public static AppError FailedTransaction => new(EFCoreErrorCodes.FailedTransaction);
    // Add any other specific EF Core related errors here
    public static AppError ConcurrencyConflict => new(EFCoreErrorCodes.ConcurrencyConflict);
    public static AppError ConstraintViolation => new(EFCoreErrorCodes.ConstraintViolation);
}