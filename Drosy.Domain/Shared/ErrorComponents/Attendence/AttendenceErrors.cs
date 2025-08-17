using Drosy.Domain.Shared.ErrorComponents.Sesstions;

namespace Drosy.Domain.Shared.ErrorComponents.Attendence
{
    /// <summary>
    /// Provides AppError instances for attendance-specific errors.
    /// </summary>
    public static class AttendenceErrors
    {
        public static AppError NotFound => new(AttendenceErrorCodes.AttendenceNotFound);
        public static AppError AlreadyExists => new(AttendenceErrorCodes.AlreadyExists);
        public static AppError InvalidStudent => new(AttendenceErrorCodes.InvalidStudentId);
        public static AppError InvalidSession => new(AttendenceErrorCodes.InvalidSessionId);
        public static AppError InvalidStatus => new(AttendenceErrorCodes.InvalidStatus);
        public static AppError CannotSaveChanges => new(AttendenceErrorCodes.CannotSaveChanges);
        public static AppError ConflictOnBatchAdd => new(AttendenceErrorCodes.ConflictOnBatchAdd);
    }
}
