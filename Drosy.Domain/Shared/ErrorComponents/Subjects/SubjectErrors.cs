using Drosy.Domain.Shared.Http;

namespace Drosy.Domain.Shared.ErrorComponents.Subjects;

public static class SubjectErrors
{
    #region Validations
    public static AppError NameRequired => new(SubjectErrorCodes.NameRequired);
    public static AppError IsDuplicate => new(SubjectErrorCodes.IsDuplicate);
    #endregion

    #region Data
    public static AppError SubjectNotFound => new(SubjectErrorCodes.SubjectNotFound, HttpStatus.NotFound);
    public static AppError SubjectSaveFailure => new(SubjectErrorCodes.SubjectSaveFailure);
    public static AppError SubjectDeleteFailure => new(SubjectErrorCodes.SubjectDeleteFailure);
    public static AppError SubjectFailure => new(SubjectErrorCodes.GeneralFailure);
    #endregion
}
