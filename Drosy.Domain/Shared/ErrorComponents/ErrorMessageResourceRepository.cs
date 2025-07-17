using Drosy.Domain.Shared.ErrorComponents.Common;
using Drosy.Domain.Shared.ErrorComponents.Common.Resources;
using Drosy.Domain.Shared.ErrorComponents.EFCore.Resources;
using Drosy.Domain.Shared.ErrorComponents.User.Resources;
using Drosy.Domain.Shared.ErrorComponents.Validation.Resources;
using System.Globalization;
using System.Resources;

namespace Drosy.Domain.Shared.ErrorComponents
{
    /// <summary>
    /// Provides localized error messages from multiple domain-specific resource files.
    /// </summary>
    public static class ErrorMessageResourceRepository
    {
        private static readonly List<ResourceManager> _resourceManagers = new()
        {
            ErrorMessages_Common.ResourceManager,
            ErrorMessages_User.ResourceManager,
            ErrorMessages_EFCore.ResourceManager,
            ValidationMessages.ResourceManager
            // Add more ResourceManagers here as needed
        };

        private static readonly ResourceManager _fallbackResourceManager = ErrorMessages_Common.ResourceManager;
        private const string FallbackKey = CommonErrorCodes.Unexpected;

        /// <summary>
        /// Retrieves a localized error message for the given code and language.
        /// </summary>
        /// <param name="code">Error code identifier (e.g., "Error_InvalidEmail").</param>
        /// <param name="language">Language code (e.g., "en", "ar").</param>
        /// <returns>Localized message if found; otherwise, a fallback message.</returns>
        public static string GetMessage(string code, string language)
        {
            var culture = GetCulture(language);

            // Try to find the message in the requested language
            foreach (var rm in _resourceManagers)
            {
                var message = rm.GetString(code, culture);
                if (!string.IsNullOrWhiteSpace(message))
                    return message;
            }

            // Fallback to English if not found
            if (!culture.TwoLetterISOLanguageName.Equals("en", StringComparison.OrdinalIgnoreCase))
            {
                var englishCulture = CultureInfo.GetCultureInfo("en");
                foreach (var rm in _resourceManagers)
                {
                    var message = rm.GetString(code, englishCulture);
                    if (!string.IsNullOrWhiteSpace(message))
                        return message;
                }
            }

            // Final fallback: generic unexpected error
            return _fallbackResourceManager.GetString(FallbackKey, CultureInfo.InvariantCulture)
                ?? "An unexpected error occurred.";
        }

        private static CultureInfo GetCulture(string language)
        {
            try
            {
                return CultureInfo.GetCultureInfo(language);
            }
            catch (CultureNotFoundException)
            {
                return CultureInfo.GetCultureInfo("en");
            }
        }
    }
}
