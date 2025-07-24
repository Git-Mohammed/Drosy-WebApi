using Drosy.Api.Commons.Responses;
using Drosy.Application.UseCases.Attendences.DTOs;
using Drosy.Application.UseCases.Attendences.Interfaces;
using Drosy.Domain.Entities;
using Drosy.Domain.Enums;
using Drosy.Domain.Shared.ApplicationResults;
using Drosy.Domain.Shared.ErrorComponents;
using Drosy.Domain.Shared.ErrorComponents.Common;
using Microsoft.AspNetCore.Mvc;
using System.Threading;

namespace Drosy.Api.Controllers
{
    [Route("api/sessions/{sessionId:int}/attendeces")]
    [ApiController]
    public class AttendencesController : ControllerBase
    {
        private readonly IAttendencesService _attendencesService;
        public AttendencesController(IAttendencesService attendencesService)
        {
            _attendencesService = attendencesService;
        }


        /// <summary>
        /// Gets a single attendance record by session and student IDs.
        /// </summary>
        /// <param name="sessionId">Identifier of the session.</param>
        /// <param name="studentId">Identifier of the student.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>
        /// 200 OK with the <see cref="AttendenceDto"/> on success.  
        /// 400 Bad Request if <paramref name="studentId"/> is less than 1.  
        /// 404 Not Found if no record exists.  
        /// 422 Unprocessable Entity for other domain validation failures.  
        /// 500 Internal Server Error on unhandled exceptions.
        /// </returns>
        [HttpGet( Name = "GetAttendenceById")]
        [ProducesResponseType(typeof(ApiResponse<AttendenceDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 422)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> GetByIdAsync([FromRoute] int sessionId, [FromQuery] int studentId, CancellationToken ct)
        {
            if (studentId < 1)
            {
                var error = new ApiError("studentId", ErrorMessageResourceRepository.GetMessage(CommonErrors.Invalid.Message, AppError.CurrentLanguage));
                return ApiResponseFactory.BadRequestResponse("studentId", "Invalid student ID.", error.Message);
            }

            try
            {
                var result = await _attendencesService.GetByIdAsync(sessionId, studentId, ct);
                if (result.IsFailure)
                    return ApiResponseFactory.FromFailure(result, nameof(GetByIdAsync), "Attendence");

                return ApiResponseFactory.SuccessResponse(result.Value);
            }
            catch (Exception ex)
            {
                return ApiResponseFactory.FromException(ex);
            }
        }


        /// <summary>
        /// Retrieves all attendance records for a given session.
        /// </summary>
        /// <param name="sessionId">Identifier of the session.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>
        /// 200 OK with a list of <see cref="AttendenceDto"/> and total count.  
        /// 422 Unprocessable Entity for domain failures.  
        /// 500 Internal Server Error on exceptions.
        /// </returns>
        [HttpGet(Name = "GetSessionAttendences")]
        [ProducesResponseType(typeof(ApiResponse<DataResult<AttendenceDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 422)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> GetAllForSessionAsync([FromRoute] int sessionId, CancellationToken ct)
        {
            try
            {
                var result = await _attendencesService.GetAllForSessionAsync(sessionId, ct);
                if (result.IsFailure)
                    return ApiResponseFactory.FromFailure(result, nameof(GetAllForSessionAsync), "Attendences");

                return ApiResponseFactory.SuccessResponse(result.Value);
            }
            catch (Exception ex)
            {
                return ApiResponseFactory.FromException(ex);
            }
        }


        /// <summary>
        /// Retrieves all attendance records for a specific student in a session.
        /// </summary>
        /// <param name="sessionId">Identifier of the session.</param>
        /// <param name="studentId">Identifier of the student.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>
        /// 200 OK with a list of <see cref="AttendenceDto"/> and total count.  
        /// 400 Bad Request if <paramref name="studentId"/> is invalid.  
        /// 422 Unprocessable Entity for other failures.  
        /// 500 Internal Server Error on exceptions.
        /// </returns>
        [HttpGet("student", Name = "GetSessionStudentAttendences")]
        [ProducesResponseType(typeof(ApiResponse<DataResult<AttendenceDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 422)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> GetAllForStudentInSessionAsync([FromRoute] int sessionId, [FromQuery] int studentId, CancellationToken ct)
        {
            if (studentId < 1)
            {
                var error = new ApiError("studentId", ErrorMessageResourceRepository.GetMessage(CommonErrors.Invalid.Message, AppError.CurrentLanguage));
                return ApiResponseFactory.BadRequestResponse("studentId", "Invalid student ID.", error.Message);
            }
            try
            {
                var result = await _attendencesService.GetAllForStudentAsync(sessionId, studentId, ct);
                if (result.IsFailure)
                    return ApiResponseFactory.FromFailure(result, nameof(GetAllForStudentInSessionAsync), "Attendences");

                return ApiResponseFactory.SuccessResponse(result.Value);
            }
            catch (Exception ex)
            {
                return ApiResponseFactory.FromException(ex);
            }
        }


        /// <summary>
        /// Retrieves attendance records filtered by status for a session.
        /// </summary>
        /// <param name="sessionId">Identifier of the session.</param>
        /// <param name="status">Attendance status to filter by.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>
        /// 200 OK with filtered <see cref="AttendenceDto"/> list.  
        /// 400 Bad Request if <paramref name="status"/> is invalid.  
        /// 422 Unprocessable Entity for service errors.  
        /// 500 Internal Server Error on exceptions.
        /// </returns>
        [HttpGet("status", Name = "GetSessionAttendencesByStatus")]
        [ProducesResponseType(typeof(ApiResponse<DataResult<AttendenceDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 422)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> GetAllForSessionByStatusAsync([FromRoute] int sessionId,  [FromQuery] AttendenceStatus status, CancellationToken ct)
        {
            if (status == null || !Enum.IsDefined(typeof(AttendenceStatus), status))
            {
                var error = new ApiError(
                    "status",
                    ErrorMessageResourceRepository.GetMessage(CommonErrors.Invalid.Message, AppError.CurrentLanguage)
                );
                return ApiResponseFactory.BadRequestResponse("status", "Invalid attendence status.", error.Message);
            }

            try
            {
                var result = await _attendencesService.GetAllForSessionByStatusAsync(sessionId,  status, ct);
                if (result.IsFailure)
                    return ApiResponseFactory.FromFailure(result, nameof(GetAllForSessionByStatusAsync), "Attendences");

                return ApiResponseFactory.SuccessResponse(result.Value);
            }
            catch (Exception ex)
            {
                return ApiResponseFactory.FromException(ex);
            }
        }


        /// <summary>
        /// Creates a new attendance record for a student in a session.
        /// </summary>
        /// <param name="sessionId">Identifier of the session.</param>
        /// <param name="dto">Attendance creation DTO.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>
        /// 201 Created with the new <see cref="AttendenceDto"/>.  
        /// 400 Bad Request if <paramref name="dto"/> is null or invalid.  
        /// 409 Conflict if a record already exists.  
        /// 422 Unprocessable Entity for service errors.  
        /// 500 Internal Server Error on exceptions.
        /// </returns>
        [HttpPost(Name = "AddStudentAttendencesForSession")]
        [ProducesResponseType(typeof(ApiResponse<AttendenceDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 409)]
        [ProducesResponseType(typeof(ApiResponse<object>), 422)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> AddAsync([FromRoute] int sessionId, [FromBody] AddAttendencenDto dto, CancellationToken ct)
        {
            try
            {
                if (dto == null)
                {
                    var error = new ApiError("dto", ErrorMessageResourceRepository.GetMessage(CommonErrors.NullValue.Message, AppError.CurrentLanguage));
                    return ApiResponseFactory.BadRequestResponse("dto", "Invalid attendence data.", error.Message);
                }

                var result = await _attendencesService.AddAsync(sessionId, dto, ct);

                if (result.IsFailure)
                {
                    return ApiResponseFactory.FromFailure(result, nameof(AddAsync), "Attendence");
                }

                return ApiResponseFactory.CreatedResponse(
                   "GetAttendencetById",
                    new { sessionId, id = result.Value.StudentId },
                    result.Value, "Attendence added for session successfully."
                );
            }
            catch (Exception ex)
            {
                return ApiResponseFactory.FromException(ex);
            }
        }


        /// <summary>
        /// Creates multiple attendance records for students in a session.
        /// </summary>
        /// <param name="sessionId">Identifier of the session.</param>
        /// <param name="dtos">Collection of attendance creation DTOs.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>
        /// 201 Created with a <see cref="DataResult{AttendenceDto}"/> containing created records and count.  
        /// 400 Bad Request if <paramref name="dtos"/> is null or empty.  
        /// 409 Conflict if all provided records already exist.  
        /// 422 Unprocessable Entity for other domain errors.  
        /// 500 Internal Server Error on exceptions.
        /// </returns>
        [HttpPost("batch", Name = "AddRangeOfStudentAttendenceForSession")]
        [ProducesResponseType(typeof(ApiResponse<DataResult<AttendenceDto>>), 201)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 409)]
        [ProducesResponseType(typeof(ApiResponse<object>), 422)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> AddRangeAsync([FromRoute] int sessionId, [FromBody] IEnumerable<AddAttendencenDto> dtos, CancellationToken ct)
        {
            try
            {
                if (dtos == null || !dtos.Any())
                {
                    var err = new ApiError("dtos", ErrorMessageResourceRepository.GetMessage(CommonErrors.NullValue.Message, AppError.CurrentLanguage));
                    return ApiResponseFactory.BadRequestResponse("dtos", err.Message, err.Message);
                }

                var result = await _attendencesService.AddRangeAsync(sessionId, dtos, ct);

                if (result.IsFailure)
                {
                    return ApiResponseFactory.FromFailure(result, nameof(AddRangeAsync), "Attendence");
                }

                return ApiResponseFactory.CreatedResponse(
                   "GetSessionAttendences",
                   new { sessionId },
                   result.Value, "Attendences added for session successfully."
                );
            }
            catch (Exception ex)
            {
                return ApiResponseFactory.FromException(ex);
            }
        }



        /// <summary>
        /// Updates an existing attendance record for a student in a session.
        /// </summary>
        /// <param name="sessionId">Identifier of the session.</param>
        /// <param name="studentId">Identifier of the student.</param>
        /// <param name="dto">Attendance update DTO.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>
        /// 200 OK on successful update.  
        /// 400 Bad Request if <paramref name="studentId"/> is less than 1 or <paramref name="dto"/> is null.  
        /// 404 Not Found if the record does not exist.  
        /// 422 Unprocessable Entity for domain validation errors.  
        /// 500 Internal Server Error on exceptions.
        /// </returns>
        [HttpPut( Name = "UpdateAttendence")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 422)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> UpdateAsync([FromRoute] int sessionId, [FromQuery] int studentId, [FromBody] UpdateAttendencenDto dto, CancellationToken ct)
        {
            if (studentId < 1)
            {
                var error = new ApiError("studentId", ErrorMessageResourceRepository.GetMessage(CommonErrors.Invalid.Message, AppError.CurrentLanguage));
                return ApiResponseFactory.BadRequestResponse("studentId", "Invalid student ID.", error.Message);
            }

            if (dto == null)
            {
                var error = new ApiError("dto", ErrorMessageResourceRepository.GetMessage(CommonErrors.NullValue.Message, AppError.CurrentLanguage));
                return ApiResponseFactory.BadRequestResponse("dto", "Invalid student attendence data.", error.Message);
            }

            try
            {
                var result = await _attendencesService.UpdateAsync(sessionId, studentId , dto, ct);

                if (result.IsFailure)
                {
                    return ApiResponseFactory.FromFailure(result, nameof(UpdateAsync), "Attendence");
                }

                return ApiResponseFactory.SuccessResponse("Student Attendence updated successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponseFactory.FromException(ex);
            }
        }


        /// <summary>
        /// Deletes an attendance record for a student in a session.
        /// </summary>
        /// <param name="sessionId">Identifier of the session.</param>
        /// <param name="studentId">Identifier of the student.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>
        /// 200 OK on successful deletion.  
        /// 400 Bad Request if <paramref name="studentId"/> is less than 1.  
        /// 404 Not Found if the record does not exist.  
        /// 422 Unprocessable Entity for domain errors.  
        /// 500 Internal Server Error on exceptions.
        /// </returns>
        [HttpDelete(Name = "DeleteAttendence")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 422)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> DeleteAsync( [FromRoute] int sessionId,   [FromQuery] int studentId,  CancellationToken ct) {
     
            if (studentId < 1)
            {
                var error = new ApiError(  "studentId",   ErrorMessageResourceRepository.GetMessage(CommonErrors.Invalid.Message, AppError.CurrentLanguage)    );
                return ApiResponseFactory.BadRequestResponse(     "studentId",  "Invalid student ID.",error.Message);
            }

            try {
                var result = await _attendencesService.DeleteAsync(sessionId, studentId, ct);
                if (result.IsFailure)
                    return ApiResponseFactory.FromFailure(  result,    nameof(DeleteAsync), "Attendence" );

                return ApiResponseFactory.SuccessResponse("Attendence deleted successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponseFactory.FromException(ex);
            }
        }

    }
}
