namespace Drosy.Domain.Shared.ErrorComponents.User
{
    /// <summary>
    /// Defines user management specific error codes.
    /// </summary>
    public static class UserErrors
    {
        public static AppError InvalidCredentials => new("Error_User_InvalidCredentials");
        public static AppError AttemptExceeded => new("Error_User_AttemptExceeded");
        public static AppError AlreadyExists => new("Error_User_AlreadyExists");
        public static AppError Inactive => new("Error_User_Inactive");
        public static AppError Locked => new("Error_User_Locked");
    }
}