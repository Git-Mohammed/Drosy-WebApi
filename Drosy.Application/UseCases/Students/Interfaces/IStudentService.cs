using Drosy.Application.UseCases.Students.DTOs;
using Drosy.Domain.Shared.ApplicationResults;
using Drosy.Domain.Shared.ResultPattern;

namespace Drosy.Application.UseCases.Students.Interfaces
{
    /// <summary>
    /// Defines contract for student-related operations including retrieval, creation, and modification.
    /// </summary>
    public interface IStudentService
    {
        #region Read

        /// <summary>
        /// Retrieves a student by their unique identifier.
        /// </summary>
        /// <param name="id">The ID of the student to retrieve.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>
        /// A result containing the <see cref="StudentDTO"/> if found; otherwise, an error result.
        /// </returns>
        Task<Result<StudentDTO>> GetByIdAsync(int id, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves a list of student information cards.
        /// </summary>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>
        /// A result containing a list of <see cref="StudentCardInfoDTO"/> objects if successful; otherwise, an error result.
        /// </returns>
        Task<Result<List<StudentCardInfoDTO>>> GetAllStudentsInfoCardsAsync(CancellationToken cancellationToken);

        #endregion

        #region Write

        /// <summary>
        /// Adds a new student using the specified data transfer object.
        /// </summary>
        /// <param name="dto">The student data to be added.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>
        /// A result containing the newly created <see cref="StudentDTO"/> or error information if creation fails.
        /// </returns>
        Task<Result<StudentDTO>> AddAsync(AddStudentDTO dto, CancellationToken cancellationToken);

        /// <summary>
        /// Updates an existing student with the provided data.
        /// </summary>
        /// <param name="dto">The updated student data.</param>
        /// <param name="id">The ID of the student to be updated.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>
        /// A result indicating success or failure of the update operation.
        /// </returns>
        Task<Result> UpdateAsync(UpdateStudentDTO dto, int id, CancellationToken cancellationToken);

        #endregion

        #region Checks
        public Task<Result> ExistsAsync(int id, CancellationToken cancellationToken);

        #endregion
    }
}
