namespace Drosy.Domain.Shared.ErrorComponents.Subjects;

public static class SubjectErrorCodes
{
    #region Validations
    public const string NameRequired = "Error_Subject_NameRequired";
    public const string IsDuplicate = "Error_Subject_IsDuplicate";
    #endregion

    #region Data/Repository
    public const string SubjectNotFound = "Error_Subject_NotFound";
    public const string SubjectSaveFailure = "Error_Subject_SaveFailure";
    public const string SubjectDeleteFailure = "Error_Subject_DeleteFailure";
    public const string GeneralFailure = "Error_Subject_GeneralFailure";
    #endregion
}
