using Drosy.Domain.Shared.ErrorComponents.Common;
using Drosy.Domain.Shared.Http;
using System.Globalization;

namespace Drosy.Domain.Shared.ErrorComponents
{
    /// <summary>
    /// Represents a domain-level error with a code and localized message.
    /// </summary>
    public sealed record AppError(string Code, int statusCode = HttpStatus.BadRequest)
    {
        /// <summary>
        /// Gets the localized error message based on the current language.
        /// </summary>
        public string Message => LocalizedErrorMessageProvider.GetMessage(Code, CurrentLanguage);

        private static string _currentLanguage = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

        public int StatusCode => statusCode;
        /// <summary>
        /// Gets or sets the current language used for localization.
        /// </summary>
        public static string CurrentLanguage
        {
            get => _currentLanguage;
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && LocalizedErrorMessageProvider.IsValidCulture(value))
                {
                    _currentLanguage = value;
                }
            }
        }

        /// <summary>
        /// Represents a neutral or empty error.
        /// </summary>
        public static readonly AppError None = new(string.Empty);

        // Common error shortcuts (optional convenience accessors)
        public static readonly AppError Unexpected = new(CommonErrorCodes.Unexpected);
        public static readonly AppError Failure = new(CommonErrorCodes.Failure);
        public static readonly AppError Unauthorized = new(CommonErrorCodes.Unauthorized);
    }
}
