namespace Drosy.Domain.Shared.ErrorComponents.User
{
    /// <summary>
    /// Defines user management specific error codes.
    /// </summary>
    public static class UserErrors
    {
        public static AppError InvalidCredentials => new(UserErrorCodes.InvalidCredentials);
        public static AppError AttemptExceeded => new(UserErrorCodes.AttemptExceeded);
        public static AppError AlreadyExists => new(UserErrorCodes.AlreadyExists);
        public static AppError Inactive => new(UserErrorCodes.Inactive);
        public static AppError Locked => new(UserErrorCodes.Locked);
    }
}