namespace Drosy.Domain.Shared.ErrorComponents.Payments;

public static class PaymentErrorCodes
{
    #region Validations
    public const string AmountMustBePositive = "Error_Payment_AmountMustBePositive";
    public const string PaymentMethodRequired = "Error_Payment_PaymentMethodRequired";
    public const string DateRequired = "Error_Payment_DateRequired";
    public const string StudentRequired = "Error_Payment_StudentRequired";
    public const string InvalidDateRange = "Error_Payment_InvalidDateRange"; // FromDate > ToDate
    #endregion

    #region Business Rules
    public const string PaymentExceedsOutstandingBalance = "Error_Payment_ExceedsOutstandingBalance";
    public const string PaymentModificationNotAllowed = "Error_Payment_ModificationNotAllowed";
    public const string DuplicatePaymentNotAllowed = "Error_Payment_DuplicateNotAllowed";
    #endregion

    #region Data/Repository / EF Core / Constraint
    public const string PaymentNotFound = "Error_Payment_NotFound";
    public const string PaymentSaveFailure = "Error_Payment_SaveFailure";
    public const string ConstraintViolation = "Error_Payment_ConstraintViolation";
    #endregion
}
