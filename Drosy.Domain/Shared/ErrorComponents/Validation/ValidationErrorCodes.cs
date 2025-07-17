namespace Drosy.Domain.Shared.ErrorComponents.Validation
{
    /// <summary>
    /// Defines error code constants for validation-related failures.
    /// </summary>
    public static class ValidationErrorCodes
    {
        public const string RequiredField = "Error_Validation_RequiredField";
        public const string InvalidEmail = "Error_Validation_InvalidEmail";
        public const string InvalidPhone = "Error_Validation_InvalidPhone";
        public const string MaxLengthExceeded = "Error_Validation_MaxLengthExceeded";
        public const string MinLengthNotMet = "Error_Validation_MinLengthNotMet";
        public const string InvalidFormat = "Error_Validation_InvalidFormat";
        public const string InvalidFileType = "Error_Validation_InvalidFileType";
        public const string FileTooLarge = "Error_Validation_FileTooLarge";
    }
}
