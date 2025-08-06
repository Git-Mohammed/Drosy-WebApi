using Drosy.Application.UseCases.Subjects.DTOs;
using Drosy.Domain.Shared.ApplicationResults;

namespace Drosy.Application.UseCases.Subjects.Interfaces
{
    /// <summary>
    /// Defines core operations for managing subjects including creation,
    /// retrieval, update, and deletion with result encapsulation.
    /// </summary>
    public interface ISubjectService
    {
        #region Read

        /// <summary>
        /// Retrieves all subjects.
        /// </summary>
        /// <param name="ct">Cancellation token for the asynchronous operation.</param>
        /// <returns>A result containing a data set of subject DTOs or an error.</returns>
        Task<Result<DataResult<SubjectDTO>>> GetAllAsync(CancellationToken ct);

        /// <summary>
        /// Retrieves a subject by its unique identifier.
        /// </summary>
        /// <param name="id">The ID of the subject.</param>
        /// <param name="ct">Cancellation token for the asynchronous operation.</param>
        /// <returns>A result containing the subject DTO if found, otherwise an error.</returns>
        Task<Result<SubjectDTO>> GetByIdAsync(int id, CancellationToken ct);

        #endregion

        #region Write

        /// <summary>
        /// Creates a new subject using the provided data.
        /// </summary>
        /// <param name="dto">DTO containing subject creation details.</param>
        /// <param name="ct">Cancellation token for the asynchronous operation.</param>
        /// <returns>A result containing the newly created subject DTO or an error.</returns>
        Task<Result<SubjectDTO>> CreateAsync(CreateSubjectDTO dto, CancellationToken ct);

        /// <summary>
        /// Updates an existing subject.
        /// </summary>
        /// <param name="dto">DTO containing updated subject data.</param>
        /// <param name="id">The ID of the subject to update.</param>
        /// <param name="ct">Cancellation token for the asynchronous operation.</param>
        /// <returns>A result indicating success or failure of the update.</returns>
        Task<Result> UpdateAsync(UpdateSubjectDTO dto, int id, CancellationToken ct);

        /// <summary>
        /// Deletes a subject by its unique identifier.
        /// </summary>
        /// <param name="id">The ID of the subject to delete.</param>
        /// <param name="ct">Cancellation token for the asynchronous operation.</param>
        /// <returns>A result indicating success or failure of the deletion.</returns>
        Task<Result> DeleteAsync(int id, CancellationToken ct);

        #endregion
    }
}
