namespace Drosy.Infrastructure.Persistence.Errors;

using Drosy.Domain.Shared.ResultPattern.ErrorComponents;

/// <summary>
/// Defines EF Core specific error codes.
/// </summary>
public static class EFCoreErrors
{
    public static Error NoChanges => new("Error_EFCore_NoChanges");
    public static Error CanNotSaveChanges => new("Error_EFCore_CanNotSaveChanges");
    public static Error FailedTransaction => new("Error_EFCore_FailedTransaction");
    // Add any other specific EF Core related errors here
    public static Error ConcurrencyConflict => new("Error_EFCore_ConcurrencyConflict");
    public static Error ConstraintViolation => new("Error_EFCore_ConstraintViolation");
}