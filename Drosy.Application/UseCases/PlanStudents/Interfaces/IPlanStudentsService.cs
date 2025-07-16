using Drosy.Application.UseCases.PlanStudents.DTOs;
using Drosy.Domain.Shared.DataDTOs;
using Drosy.Domain.Shared.ResultPattern;

namespace Drosy.Application.UseCases.PlanStudents.Interfaces
{
    /// <summary>
    /// Defines the contract for managing student assignments to plans.
    /// </summary>
    public interface IPlanStudentsService
    {
        /// <summary>
        /// Adds a single student to the specified plan.
        /// </summary>
        /// <param name="planId">The identifier of the plan to which the student will be added.</param>
        /// <param name="dto">The data transfer object containing student details.</param>
        /// <param name="ct">A cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="Result{T}"/> containing the added <see cref="PlanStudentDto"/> if successful.</returns>
        Task<Result<PlanStudentDto>> AddStudentToPlanAsync(int planId, AddStudentToPlanDto dto, CancellationToken ct);

        /// <summary>
        /// Adds multiple students to the specified plan.
        /// </summary>
        /// <param name="planId">The identifier of the plan to which students will be added.</param>
        /// <param name="dto">A collection of DTOs containing student details.</param>
        /// <param name="c">A cancellation token to cancel the operation.</param>
        /// <returns>
        /// A <see cref="Result{T}"/> containing a <see cref="DataResult{T}"/> of <see cref="PlanStudentDto"/>
        /// representing the students that were added.
        /// </returns>
        Task<Result<DataResult<PlanStudentDto>>> AddRangeOfStudentToPlanAsync(int planId, IEnumerable<AddStudentToPlanDto> dto, CancellationToken c);
    }
}
