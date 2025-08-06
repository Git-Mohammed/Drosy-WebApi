using Drosy.Application.UseCases.Payments.DTOs;
using Drosy.Domain.Shared.ApplicationResults;
using System.Threading;
using System.Threading.Tasks;

namespace Drosy.Application.UseCases.Payments.Interfaces
{
    /// <summary>
    /// Provides operations for managing student payments.
    /// </summary>
    public interface IPaymentService
    {
        #region Read
        /// <summary>
        /// Retrieves a payment record by its unique identifier.
        /// </summary>
        /// <param name="id">The unique ID of the payment.</param>
        /// <param name="cancellation">A cancellation token to allow graceful termination of the operation.</param>
        /// <returns>
        /// A <see cref="Result{PaymentDto}"/> containing the payment data if found.  
        /// Failure if the payment does not exist or an error occurs.
        /// </returns>
        Task<Result<PaymentDto>> GetByIdAsync(int id, CancellationToken cancellation);

        /// <summary>
        /// Retrieves payment history for a specific student, optionally filtered by date, payment method, or other criteria.
        /// </summary>
        /// <param name="studentId">The unique ID of the student whose payment history is being requested.</param>
        /// <param name="filter">Filtering options such as date range or payment type.</param>
        /// <param name="cancellation">A cancellation token to support graceful request termination.</param>
        /// <returns>
        /// A <see cref="Result{StudentPaymentHistoryDTO}"/> representing the student's filtered payment history.  
        /// Failure result if the student is not found or if domain validation fails.
        /// </returns>
        Task<Result<StudentPaymentHistoryDTO>> GetStudentPaymentHistoryAsync(
            int studentId,
            PaymentHistoryFilterDTO filter,
            CancellationToken cancellation);

        #endregion

        #region Write

        /// <summary>
        /// Creates a new payment record for a student.
        /// </summary>
        /// <param name="paymentDto">The payment data transfer object containing payment details.</param>
        /// <param name="cancellation">A cancellation token to allow graceful termination of the operation.</param>
        /// <returns>
        /// A <see cref="Result{PaymentDto}"/> containing the created payment data if successful.  
        /// Failure if the DTO is invalid or a domain error occurs.
        /// </returns>
        Task<Result<PaymentDto>> CreateAsync(CreatePaymentDto paymentDto, CancellationToken cancellation);
        Task<Result> UpdateAsync(int id,UpdatePaymentDto paymentDto, CancellationToken cancellation);
        Task<Result> DeleteAsync(int id, CancellationToken ct);
        
        #endregion

    }
}
