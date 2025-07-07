namespace Drosy.Domain.Shared.ResultPattern.ErrorComponents
{
    /// <summary>
    /// Holds the localized error messages.
    /// </summary>
    public static class ErrorMessagesRepository
    {
        /// <summary>
        /// The default language code.
        /// </summary>
        private const string DefaultLanguage = "en";

        /// <summary>
        /// A read-only dictionary of localized error messages.
        /// </summary>
        private static readonly IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> LocalizedMessages =
            new Dictionary<string, IReadOnlyDictionary<string, string>>
            {
                ["en"] = new Dictionary<string, string>
                {

                    ["Error.NullValue"] = "A null value was provided.",
                    ["Error.NotFound"] = "The requested resource was not found.",
                    ["Error.Invalid"] = "The provided data is invalid.",
                    ["Error.Unauthorized"] = "Access denied. You are not authorized.",
                    ["Error.Conflict"] = "A conflict was detected with the current state of the resource.",
                    ["Error.Failure"] = "An internal failure occurred. Please try again later.",
                    ["Error.EFCore.NoChanges"] = "No Change Has Been Made To The Server",
                    ["Error.EFCore.FailedTransaction"] = "Faild To Start Trasnaction in the server.",
                    ["Error.User.InvalidCredentials"] = "Username Or Password Invalid",
                    ["Error.User.AttempExceeded"] = "Your Trails Has Been Exceeded Try Later"

                },
                ["pt"] = new Dictionary<string, string>
                {
                    ["Error.NullValue"] = "Um valor nulo foi fornecido.",
                    ["Error.NotFound"] = "O recurso solicitado não foi encontrado.",
                    ["Error.Invalid"] = "Os dados fornecidos são inválidos.",
                    ["Error.Unauthorized"] = "Acesso negado. Você não está autorizado.",
                    ["Error.Conflict"] = "Conflito detectado com o estado atual do recurso.",
                    ["Error.Failure"] = "Ocorreu uma falha interna. Tente novamente mais tarde.",
                }
                ,
                ["ar"] = new Dictionary<string, string>
                {
                    ["Error.NullValue"] = "تم تقديم قيمة فارغة.",
                    ["Error.NotFound"] = "لم يتم العثور على المورد المطلوب.",
                    ["Error.Invalid"] = "البيانات المقدمة غير صالحة.",
                    ["Error.Unauthorized"] = "تم رفض الوصول. أنت غير مصرح لك.",
                    ["Error.Conflict"] = "تم اكتشاف تعارض مع الحالة الحالية للمورد.",
                    ["Error.Failure"] = "حدث خطأ داخلي. يرجى المحاولة مرة أخرى لاحقًا.",
                    ["Error.EFCore.NoChanges"] = "لم يحدث اي تغير على السيرفر.",
                    ["Error.EFCore.FailedTransaction"] = "فشل في بدأ عملية في السيرفر.",
                    ["Error.User.InvalidCredentials"] = ".اسم المستخدم او كلمة المرور غير صحيح",
                    ["Error.User.AttempExceeded"] = "لقد نفذة محاولاتك, رجاء حاول لاحقا"
                },
            };

        /// <summary>
        /// Retrieves the localized error message for a given error code and language.
        /// </summary>
        /// <param name="code">The error code identifier.</param>
        /// <param name="language">
        /// The language code (e.g. "en", "pt"). If not found, it falls back to the default language.
        /// </param>
        /// <returns>
        /// The error message if found; otherwise, a generic "unexpected error occurred" message.
        /// </returns>
        public static string GetMessage(string code, string language)
        {
            // Attempt to get the messages table for the specified language.
            if (!LocalizedMessages.TryGetValue(language, out var messages))
            {
                messages = LocalizedMessages[DefaultLanguage];
            }

            // Attempt to retrieve the message for the provided error code.
            if (messages.TryGetValue(code, out var message))
            {
                return message;
            }

            // Fallback: if the code is not found, return a generic message.
            return "An unexpected error occurred.";
        }
    }
}