using Drosy.Domain.Shared.ErrorComponents.Payments;

namespace Drosy.Domain.Shared.ErrorComponents.Payments;

public static class PaymentErrors
{
    #region Validations
    public static AppError AmountMustBePositive => new(PaymentErrorCodes.AmountMustBePositive);
    public static AppError PaymentMethodRequired => new(PaymentErrorCodes.PaymentMethodRequired);
    public static AppError DateRequired => new(PaymentErrorCodes.DateRequired);
    public static AppError StudentRequired => new(PaymentErrorCodes.StudentRequired);
    public static AppError InvalidDateRange => new(PaymentErrorCodes.InvalidDateRange);
    #endregion

    #region Business Rules
    public static AppError PaymentExceedsOutstandingBalance => new(PaymentErrorCodes.PaymentExceedsOutstandingBalance);
    public static AppError PaymentModificationNotAllowed => new(PaymentErrorCodes.PaymentModificationNotAllowed);
    public static AppError DuplicatePaymentNotAllowed => new(PaymentErrorCodes.DuplicatePaymentNotAllowed);
    #endregion

    #region Data/Repository / EF Core / Constraint
    public static AppError PaymentNotFound => new(PaymentErrorCodes.PaymentNotFound);
    public static AppError PaymentSaveFailure => new(PaymentErrorCodes.PaymentSaveFailure);
    public static AppError ConstraintViolation => new(PaymentErrorCodes.ConstraintViolation);
    #endregion
}
