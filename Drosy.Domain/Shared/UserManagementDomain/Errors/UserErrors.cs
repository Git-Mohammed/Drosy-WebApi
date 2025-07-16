using Drosy.Domain.Shared.ResultPattern.ErrorComponents; // Ensure this namespace is correctly referenced

namespace Drosy.Domain.Shared.UserManagementDomain.Errors
{
    /// <summary>
    /// Defines user management specific error codes.
    /// </summary>
    public static class UserErrors
    {
        public static Error InvalidCredentials => new("Error_User_InvalidCredentials");
        public static Error AttemptExceeded => new("Error_User_AttemptExceeded");
        public static Error AlreadyExists => new("Error_User_AlreadyExists");
        public static Error Inactive => new("Error_User_Inactive");
        public static Error Locked => new("Error_User_Locked");
        public static Error NotFound => new("Error_User_NotFound");
        public static Error PermissionDenied => new("Error_User_PermissionDenied");
    }
}