using Drosy.Application.UseCases.Students.DTOs;
using Drosy.Domain.Entities;
using Drosy.Domain.Shared.ApplicationResults;

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

        /// <summary>
        /// Retrieves detailed information for a specific student by their ID.
        /// </summary>
        /// <param name="studentId">The unique identifier of the student.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>
        /// A result containing the student's detailed information if found, or a failure result if not.
        /// </returns>
        public Task<Result<StudentDetailsDto?>> GetStudentInfoDetailsAsync(int studentId, CancellationToken cancellationToken);


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

        /// <summary>
        /// Deletes a student record based on the specified student ID.
        /// </summary>
        /// <param name="id">The ID of the student to be deleted.</param>
        /// <param name="deletedBy">The ID of the user performing the deletion.</param>
        /// <param name="ct">Token to monitor for cancellation requests.</param>
        /// <returns>
        /// A result indicating success or failure of the deletion operation.
        /// </returns>
        Task<Result> DeleteStudentAsync(int id, int deletedBy, CancellationToken ct);


        #endregion

        #region Checks
        public Task<Result> ExistsAsync(int id, CancellationToken cancellationToken);

        #endregion
    }
}
