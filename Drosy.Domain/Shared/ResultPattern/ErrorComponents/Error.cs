namespace Drosy.Domain.Shared.ResultPattern.ErrorComponents;

using global::System.Globalization;

public record Error(string Code)
{
    // Message is fetched dynamically based on CurrentLanguage
    public string Message => ErrorMessageHandler.GetMessage(Code, CurrentLanguage);

    private static string _currentLanguage = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

    public static string CurrentLanguage
    {
        get => _currentLanguage;
        set
        {
            if (!string.IsNullOrWhiteSpace(value) && ErrorMessageHandler.IsValidCulture(value))
            {
                _currentLanguage = value;
            }
        }
    }

    public static readonly Error None = new(string.Empty);
    public static readonly Error NullValue = new("Error_NullValue");
    public static readonly Error NotFound = new("Error_NotFound");
    public static readonly Error Invalid = new("Error_Invalid");
    public static readonly Error Unauthorized = new("Error_Unauthorized");
    public static readonly Error Conflict = new("Error_Conflict");
    public static readonly Error Failure = new("Error_Failure");
    public static readonly Error OperationCancelled = new("Error_OperationCancelled");
    public static readonly Error BusinessRule = new("Error_BusinessRule");

}