namespace Drosy.Domain.Shared.ResultPattern.ErrorComponents;

using global::System.Globalization;

public static class ErrorMessageHandler
{
    public static string GetMessage(string code, string language = "en")
    {
        if (string.IsNullOrWhiteSpace(language) || !IsValidCulture(language))
        {
            language = "en"; // Fallback to default if invalid
        }
        return ErrorMessagesRepository.GetMessage(code, language);
    }

    public static string GetMessage(string code)
    {
        return GetMessage(code, "en");
    }

    public static bool IsValidCulture(string language)
    {
        try
        {
            CultureInfo.GetCultureInfo(language);
            return true;
        }
        catch (CultureNotFoundException)
        {
            return false;
        }
    }
}