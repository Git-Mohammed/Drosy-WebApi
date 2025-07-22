namespace Drosy.Domain.Shared.ErrorComponents.Sesstions
{
    /// <summary>
    /// Provides AppError instances for session-specific validation issues.
    /// </summary>
    public static class SessionErrors
    {
        public static AppError TimeOverlap => new(SessionErrorCodes.TimeOverlap);
        public static AppError StartAfterEnd => new(SessionErrorCodes.StartAfterEnd);
        public static AppError OutsideExpectedDate => new(SessionErrorCodes.OutsideExpectedDate);
        public static AppError TitleRequired => new(SessionErrorCodes.TitleRequired);
        public static AppError InvalidTimeRange => new(SessionErrorCodes.InvalidTimeRange);
        public static AppError PlanNotFound => new(SessionErrorCodes.PlanNotFound);
        public static AppError SessionNotFound => new(SessionErrorCodes.SessionNotFound); 

    }
}
