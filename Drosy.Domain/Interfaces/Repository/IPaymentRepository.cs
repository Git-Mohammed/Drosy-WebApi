using Drosy.Domain.Entities;
using Drosy.Domain.Interfaces.Common.Repository;

namespace Drosy.Domain.Interfaces.Repository;

/// <summary>
/// Provides repository operations related to student payments.
/// </summary>
public interface IPaymentRepository : IRepository<Payment>
{
    #region Read
    /// <summary>
    /// Retrieves a specific payment by its unique identifier.
    /// </summary>
    /// <param name="id">The unique payment ID.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task<Payment?> GetByIdAsync(int id, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a list of payments for the given student,
    /// optionally filtered by date range and ordered by payment date.
    /// </summary>
    /// <param name="studentId">ID of the student.</param>
    /// <param name="fromDate">Start of date filter range (optional).</param>
    /// <param name="toDate">End of date filter range (optional).</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task<List<Payment>> GetStudentPaymentsAsync(
        int studentId,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves the total fee amount expected from a given student
    /// based on their enrolled plans.
    /// </summary>
    /// <param name="studentId">ID of the student.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task<decimal> GetStudentTotalFeeAsync(int studentId, CancellationToken cancellationToken);

    #endregion
}
