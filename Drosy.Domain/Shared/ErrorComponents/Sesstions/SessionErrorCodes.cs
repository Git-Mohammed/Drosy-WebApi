namespace Drosy.Domain.Shared.ErrorComponents.Sesstions
{
    /// <summary>
    /// Contains error code constants for session-specific validation failures.
    /// </summary>
    public static class SessionErrorCodes
    {
        public const string TimeOverlap = "Error_Session_TimeOverlap";
        public const string StartAfterEnd = "Error_Session_StartAfterEnd";
        public const string OutsideExpectedDate = "Error_Session_OutsideExpectedDate";
        public const string TitleRequired = "Error_Session_TitleRequired";
        public const string InvalidTimeRange = "Error_Session_InvalidTimeRange";
        public const string PlanNotFound = "Error_Session_PlanNotFound";
        public const string SessionNotFound = "Error_Session_SessionNotFound";
        public const string ExpectedDateInThePast = "Error_Session_ExpectedDateInThePast";
    }
}
