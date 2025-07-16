namespace Drosy.Domain.Shared.ErrorComponents;

using global::System.Globalization;
using global::System.Resources;
using Drosy.Domain.Shared.ErrorComponents.Common.Resources;
using Drosy.Domain.Shared.ErrorComponents.User.Resources;
using Drosy.Domain.Shared.ErrorComponents.EFCore.Resources;

public static class ErrorMessageResourceRepository
{
    // List of all ResourceManagers for different error domains
    private static readonly List<ResourceManager> _resourceManagers = new()
    {
        // Common Errors (now definitively ErrorMessages_Common.ResourceManager)
        ErrorMessages_Common.ResourceManager,

        // User Errors (assuming ErrorMessages.User is generated in Drosy.Domain.Shared.UserManagementDomain.Resources
        // and Drosy.Domain.Shared references this project)
        ErrorMessages_User.ResourceManager,

        // EFCore Errors (NOW MOVED to Drosy.Domain.Shared, so it's directly accessible)
        // No Assembly.Load() needed here anymore for EFCore resources.
        ErrorMessages_EFCore.ResourceManager
        // Add more ResourceManagers here if you introduce other error domains following this pattern
    };

    // Dedicated ResourceManager for the "Unexpected Error" fallback
    private static readonly ResourceManager _commonResourceManager =
        ErrorMessages_Common.ResourceManager;


    /// <summary>
    /// Retrieves the localized error message for a given error code and language.
    /// </summary>
    /// <param name="code">The error code identifier (e.g., "Error_NullValue").</param>
    /// <param name="language">The language code (e.g., "en", "ar").</param>
    /// <returns>The error message if found; otherwise, a generic "unexpected error occurred" message.</returns>
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
        // Re-attempt using English culture across all resource managers
        if (string.IsNullOrEmpty(message) && !language.Equals("en", StringComparison.OrdinalIgnoreCase))
        {
            CultureInfo englishCulture = CultureInfo.GetCultureInfo("en");
            foreach (var rm in _resourceManagers)
            {
                message = rm.GetString(code, englishCulture);
                if (!string.IsNullOrEmpty(message))
                {
                    return message; // Found the message in English
                }
            }
        }

        // Final fallback: return the generic "Unexpected Error" message from the common resources
        return _commonResourceManager.GetString("Error_Unexpected", CultureInfo.InvariantCulture) ?? "An unexpected error occurred.";
    }
}