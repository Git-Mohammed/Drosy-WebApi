namespace Drosy.Domain.Shared.ErrorComponents.Plans;

public static class PlanErrorCodes
{
    #region Validations
    
    public const string StartDateRequired = "Error_Plan_StartDateRequired";
    public const string EndDateRequired = "Error_Plan_EndDateRequired";
    public const string StartSessionRequired = "Error_Plan_StartSessionRequired";
    public const string EndSessionRequired = "Error_Plan_EndSessionRequired";
    public const string TotalFeesMustBePositive = "Error_Plan_TotalFeesMustBePositive";
    public const string InvalidSessionTimeRange = "Error_Plan_InvalidSessionTimeRange"; // start > end
    public const string InvalidDateRange = "Error_Plan_InvalidDateRange"; // start > end
    public const string DaysOfWeekRequired = "Error_Plan_DaysOfWeekRequired";
    
    #endregion
    
    #region Business Rule
    public const string PlanTypeNotSupported = "Error_Plan_TypeNotSupported";
    public const string PlanInactive = "Error_Plan_Inactive";
    public const string SessionTimeConflict = "Error_Plan_SessionTimeConflict";
    #endregion
    
    #region Data/Repository / EF Core / Constraint
    
    public const string PlanNotFound = "Error_Plan_NotFound";
    public const string PlanCannotBeDeletedWithStudents = "Error_Plan_CannotDeleteWithStudents"; // لها طلاب مسجلين
    public const string PlanSaveFailure = "Error_Plan_SaveFailure";
    public const string ConstraintViolation = "Error_Plan_ConstraintViolation";
    
    #endregion
}