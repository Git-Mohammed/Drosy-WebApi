namespace Drosy.Domain.Shared.ErrorComponents.Attendence
{
    /// <summary>
    /// Contains error code constants for attendance-specific validation and domain failures.
    /// </summary>
    public static class AttendenceErrorCodes
    {
        public const string AttendenceNotFound = "Error_Attendence_NotFound";
        public const string AlreadyExists = "Error_Attendence_AlreadyExists";
        public const string InvalidStudentId = "Error_Attendence_InvalidStudentId";
        public const string InvalidSessionId = "Error_Attendence_InvalidSessionId";
        public const string InvalidStatus = "Error_Attendence_InvalidStatus";
        public const string CannotSaveChanges = "Error_Attendence_CannotSaveChanges";
        public const string ConflictOnBatchAdd = "Error_Attendence_ConflictOnBatchAdd";
    }
}
