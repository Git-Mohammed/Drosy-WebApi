namespace Drosy.Domain.Shared.ResultPattern.ErrorComponents;

using global::System.Globalization;
using global::System.Reflection;
using global::System.Resources;

// IMPORTANT: Ensure these using statements correctly point to your generated resource classes.
// The exact class names (e.g., ErrorMessages_Common_en) depend on how ResXFileCodeGenerator names them
// when you specify the culture in the .resx filename.
using Drosy.Domain.Shared.ResultPattern.ErrorComponents.Resources; // Might contain generated classes directly or nested
using Drosy.Domain.Shared.ResultPattern.ErrorComponents.Resources.ErrorMessages_Common_en; // For the English common errors
using Drosy.Domain.Shared.ResultPattern.ErrorComponents.Resources.ErrorMessages_Common_ar; // For the Arabic common errors
using Drosy.Domain.Shared.UserManagementDomain.Resources; // For user errors (assuming it generates ErrorMessages.User)
using Drosy.Infrastructure.Persistence.Errors.Resources; // For EFCore errors (assuming it generates ErrorMessages.EFCore)

public static class ErrorMessagesRepository
{
    // List of all ResourceManagers for different error domains
    private static readonly List<ResourceManager> _resourceManagers = new()
    {
        // Common Errors - English (explicitly from ErrorMessages.Common.en.resx)
        // You'll need to use the exact name of the generated class for this .resx file.
        // It's often ErrorMessages_Common_en if the file is ErrorMessages.Common.en.resx
        ErrorMessages_Common.ResourceManager,

        // User Errors (assuming Drosy.UserManagementDomain project's ErrorMessages.User.resx)
        ErrorMessages_User.ResourceManager,

        // EFCore Errors (assuming Drosy.Infrastructure.Persistence.Errors project's ErrorMessages.EFCore.resx)
        ErrorMessages_EFCore.ResourceManager
        // Add more ResourceManagers for other domains/modules
    };

    // Dedicated ResourceManager for the "Unexpected Error" fallback.
    // It must point to the *English* common errors if that's your primary fallback.
    private static readonly ResourceManager _commonResourceManager =
        ErrorMessages_Common_en.ResourceManager; // Ensure this points to your default/fallback language resource


    public static string GetMessage(string code, string language)
    {
        CultureInfo culture = CultureInfo.GetCultureInfo(language);
        string? message = null;

        // First attempt: try to find the message in the specified language across all resource managers
        foreach (var rm in _resourceManagers)
        {
            message = rm.GetString(code, culture);
            if (!string.IsNullOrEmpty(message))
            {
                return message; // Found the message in the requested language
            }
        }

        // Second attempt: if not found in the specified language, try to find it in English (default fallback)
        // This specifically targets the English resources for a final fallback before generic.
        if (string.IsNullOrEmpty(message) && !language.Equals("en", StringComparison.OrdinalIgnoreCase))
        {
            CultureInfo englishCulture = CultureInfo.GetCultureInfo("en");
            // You can simplify this by directly checking ErrorMessages_Common_en.ResourceManager
            // or iterate through _resourceManagers again with the English culture.
            message = ErrorMessages_Common_en.ResourceManager.GetString(code, englishCulture);
            if (!string.IsNullOrEmpty(message))
            {
                return message; // Found the message in English fallback
            }
        }

        // Final fallback: return the generic "Unexpected Error" message from the common English resources
        return _commonResourceManager.GetString("Error_Unexpected", CultureInfo.InvariantCulture) ?? "An unexpected error occurred.";
    }
}