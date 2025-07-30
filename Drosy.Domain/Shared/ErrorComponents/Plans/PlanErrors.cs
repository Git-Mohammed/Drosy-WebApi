namespace Drosy.Domain.Shared.ErrorComponents.Plans;

public static class PlanErrors
{
    #region Validations

    public static AppError StartDateRequired => new(PlanErrorCodes.StartDateRequired);
    public static AppError EndDateRequired => new(PlanErrorCodes.EndDateRequired);
    public static AppError StartSessionRequired => new(PlanErrorCodes.StartSessionRequired);
    public static AppError EndSessionRequired => new(PlanErrorCodes.EndSessionRequired);
    public static AppError TotalFeesMustBePositive => new(PlanErrorCodes.TotalFeesMustBePositive);
    public static AppError InvalidSessionTimeRange => new(PlanErrorCodes.InvalidSessionTimeRange);
    public static AppError InvalidDateRange => new(PlanErrorCodes.InvalidDateRange);
    public static AppError DaysOfWeekRequired => new(PlanErrorCodes.DaysOfWeekRequired);
    public static AppError InvalidPeriod => new(PlanErrorCodes.InvalidPeriod);

    #endregion

    #region Business Rules

    public static AppError PlanTypeNotSupported => new(PlanErrorCodes.PlanTypeNotSupported);
    public static AppError PlanInactive => new(PlanErrorCodes.PlanInactive);
    public static AppError SessionTimeConflict => new(PlanErrorCodes.SessionTimeConflict);

    #endregion

    #region Data/Repository / EF Core / Constraint

    public static AppError PlanNotFound => new(PlanErrorCodes.PlanNotFound);
    public static AppError PlanCannotBeDeletedWithStudents => new(PlanErrorCodes.PlanCannotBeDeletedWithStudents);
    public static AppError PlanSaveFailure => new(PlanErrorCodes.PlanSaveFailure);
    public static AppError ConstraintViolation => new(PlanErrorCodes.ConstraintViolation);

    #endregion

    public static AppError PlanDeleteFailure { get; set; }
}