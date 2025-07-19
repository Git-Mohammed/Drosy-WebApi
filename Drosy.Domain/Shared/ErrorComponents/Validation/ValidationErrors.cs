namespace Drosy.Domain.Shared.ErrorComponents.Validation
{
    /// <summary>
    /// Provides domain-level AppError instances for validation failures.
    /// </summary>
    public static class ValidationErrors
    {
        public static AppError RequiredField => new(ValidationErrorCodes.RequiredField);
        public static AppError InvalidEmail => new(ValidationErrorCodes.InvalidEmail);
        public static AppError InvalidPhone => new(ValidationErrorCodes.InvalidPhone);
        public static AppError MaxLengthExceeded => new(ValidationErrorCodes.MaxLengthExceeded);
        public static AppError MinLengthNotMet => new(ValidationErrorCodes.MinLengthNotMet);
        public static AppError InvalidFormat => new(ValidationErrorCodes.InvalidFormat);
        public static AppError InvalidFileType => new(ValidationErrorCodes.InvalidFileType);
        public static AppError FileTooLarge => new(ValidationErrorCodes.FileTooLarge);
    }
}
