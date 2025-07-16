namespace Drosy.Domain.Shared.ErrorComponents.EFCoreErrors
{
    public static class EFCoreErrorCodes
    {
        public const string NoChanges = "Error_EFCore_NoChanges";
        public const string CanNotSaveChanges = "Error_EFCore_CanNotSaveChanges";
        public const string FailedTransaction = "Error_EFCore_FailedTransaction";
        public const string ConcurrencyConflict = "Error_EFCore_ConcurrencyConflict";
        public const string ConstraintViolation = "Error_EFCore_ConstraintViolation";
    }
}
