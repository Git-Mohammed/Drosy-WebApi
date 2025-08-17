using System.Security.Claims;
using Drosy.Api.Commons.Responses;
using Drosy.Application.UseCases.Students.DTOs;
using Drosy.Application.UseCases.Students.Interfaces;
using Drosy.Domain.Shared.ApplicationResults;
using Drosy.Domain.Shared.ErrorComponents;
using Drosy.Domain.Shared.ErrorComponents.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Drosy.Api.Controllers
{
    // TODO:
    // - Athourization
    // - add canclelation token to all async methods

    [ApiController]
    [Route("api/students")]
    [Authorize]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;
        public StudentsController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        #region Read
        /// <summary>
        /// Retrieves the student entity with the specified ID.
        /// </summary>
        /// <param name="id">The unique identifier of the student.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the student details if found; 
        /// otherwise, an appropriate error response.
        /// </returns>
        [HttpGet("{id:int}", Name = "GetStudentByIdAsync")]
        public async Task<IActionResult> GetByIdAsync(int id, CancellationToken ct)
        {
            if (id < 1)
            {
                var error = new ApiError("id", ErrorMessageResourceRepository.GetMessage(CommonErrors.Invalid.Message, AppError.CurrentLanguage));
                return ApiResponseFactory.BadRequestResponse("id", "Invalid student ID.", error.Message);
            }
            try
            {
                var result = await _studentService.GetByIdAsync(id, ct);

                if (result.IsFailure)
                {
                    return ApiResponseFactory.FromFailure(result, nameof(GetByIdAsync));
                }

                return ApiResponseFactory.SuccessResponse(result.Value, "Student retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponseFactory.FromException(ex);
            }
        }

        /// <summary>
        /// Retrieves a list of student information cards asynchronously.
        /// </summary>
        /// <remarks>
        /// This endpoint fetches detailed information cards for all students in the system.  
        /// It supports cancellation via the provided <paramref name="ct"/> token.  
        /// If the operation is canceled before completion, a cancellation response is returned.  
        /// If the service returns a failure, an error response is returned.
        /// </remarks>
        /// <param name="ct">A cancellation token to cancel the operation if needed.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing either:
        /// <list type="bullet">
        ///     <item><description><c>200 OK</c> with a list of <see cref="StudentCardInfoDTO"/> if successful.</description></item>
        ///     <item><description><c>400 Bad Request</c> if the operation fails due to service error.</description></item>
        ///     <item><description><c>499 Client Closed Request</c> (custom) if the request is canceled by the client.</description></item>
        /// </list>
        /// </returns>
        /// <response code="200">Returns the list of student info cards.</response>
        /// <response code="400">If the service layer returns a failure result.</response>
        /// <response code="499">If the request was canceled via the cancellation token.</response>
        [HttpGet(Name = "GetAllStudentsInfoCardsAsync")]
        public async Task<IActionResult> GetAllStudentsInfoCardsAsync(CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
            {
                return ApiResponseFactory.FromFailure(Result.Failure(CommonErrors.OperationCancelled), nameof(GetAllStudentsInfoCardsAsync));
            }

            var result = await _studentService.GetAllStudentsInfoCardsAsync(ct);

            if (result.IsFailure)
            {
                return ApiResponseFactory.FromFailure(result, nameof(GetAllStudentsInfoCardsAsync));
            }
            return ApiResponseFactory.SuccessResponse<List<StudentCardInfoDTO>>(result.Value);
        }

        [HttpGet("{studentId:int}/details", Name = "GetStudentInfoDetailsAsync")]
        public async Task<IActionResult> GetStudentInfoDetailsAsync(int studentId, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
            {
                return ApiResponseFactory.FromFailure(Result.Failure(CommonErrors.OperationCancelled), nameof(GetStudentInfoDetailsAsync));
            }

            var result = await _studentService.GetStudentInfoDetailsAsync(studentId, ct);

            if (result.IsFailure)
            {
                return ApiResponseFactory.FromFailure(result, nameof(GetAllStudentsInfoCardsAsync));
            }
            return ApiResponseFactory.SuccessResponse(result.Value);
        }



        #endregion

        #region Write
        /// <summary>
        /// Adds a new student using the provided data transfer object.
        /// </summary>
        /// <param name="dto">The student data to be added.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating success with the new student's ID or failure details.
        /// </returns>
        [HttpPost(Name = "AddStudentAsync")]
        public async Task<IActionResult> AddAsync([FromBody] AddStudentDTO dto, CancellationToken ct)
        {
            if (dto == null)
            {
                var error = new ApiError("dto", ErrorMessageResourceRepository.GetMessage(CommonErrors.NullValue.Message, AppError.CurrentLanguage));
                return ApiResponseFactory.BadRequestResponse("dto", "Invalid student data.", error.Message);
            }

            try
            {
                var result = await _studentService.AddAsync(dto, ct);

                if (result.IsFailure)
                {
                    return ApiResponseFactory.FromFailure(result, nameof(AddAsync), "Student");
                }

                return ApiResponseFactory.CreatedResponse(
                   "GetStudentByIdAsync",
                    new { id = result.Value.Id },
                    result.Value, "Student added successfully."
                );
            }
            catch (Exception ex)
            {
                return ApiResponseFactory.FromException(ex);
            }
        }

        /// <summary>
        /// Updates an existing student with the provided data transfer object and ID.
        /// </summary>
        /// <param name="dto">The updated student data.</param>
        /// <param name="id">The unique identifier of the student to be updated.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the update operation.
        /// </returns>
        [HttpPut("{id:int}", Name = "UpdateStudentAsync")]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateStudentDTO dto, int id, CancellationToken cancellationToken)
        {
            if (id < 1)
            {
                var error = new ApiError("id", ErrorMessageResourceRepository.GetMessage(CommonErrors.Invalid.Message, AppError.CurrentLanguage));
                return ApiResponseFactory.BadRequestResponse("id", "Invalid student ID.", error.Message);
            }

            if (dto == null)
            {
                var error = new ApiError("dto", ErrorMessageResourceRepository.GetMessage(CommonErrors.NullValue.Message, AppError.CurrentLanguage));
                return ApiResponseFactory.BadRequestResponse("dto", "Invalid student data.", error.Message);
            }

            try
            {
                var result = await _studentService.UpdateAsync(dto, id, cancellationToken);

                if (result.IsFailure)
                {
                    return ApiResponseFactory.FromFailure(result, nameof(UpdateAsync), "Student");
                }

                return ApiResponseFactory.SuccessResponse("Student updated successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponseFactory.FromException(ex);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteStudentAsync(int id, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
            {
                return ApiResponseFactory.FromFailure(Result.Failure(CommonErrors.OperationCancelled), nameof(DeleteStudentAsync));
            }
            var userId = 0;
            int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out userId);
            var result = await _studentService.DeleteStudentAsync(id, userId, ct);

            return result.IsSuccess ? ApiResponseFactory.SuccessResponse() : ApiResponseFactory.FromFailure(result, nameof(DeleteStudentAsync));
        }
        #endregion
    }
}
