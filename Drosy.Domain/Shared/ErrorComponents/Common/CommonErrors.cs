using Drosy.Domain.Shared.ErrorComponents;
using Drosy.Domain.Shared.ErrorComponents.Common;
using Drosy.Domain.Shared.ResultPattern.ErrorComponents;

namespace Drosy.Domain.Shared.ResultPattern.ErrorComponents.Common
{
    /// <summary>
    /// Provides domain-level AppError instances for common error scenarios.
    /// </summary>
    public static class CommonErrors
    {
        public static AppError NullValue => new(CommonErrorCodes.NullValue);
        public static AppError NotFound => new(CommonErrorCodes.NotFound);
        public static AppError Invalid => new(CommonErrorCodes.Invalid);
        public static AppError Unauthorized => new(CommonErrorCodes.Unauthorized);
        public static AppError Conflict => new(CommonErrorCodes.Conflict);
        public static AppError Failure => new(CommonErrorCodes.Failure);
        public static AppError OperationCancelled => new(CommonErrorCodes.OperationCancelled);
        public static AppError BusinessRule => new(CommonErrorCodes.BusinessRule);
        public static AppError Unexpected => new(CommonErrorCodes.Unexpected);
    }
}
