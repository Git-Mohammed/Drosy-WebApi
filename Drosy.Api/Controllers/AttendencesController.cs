using Drosy.Api.Commons.Responses;
using Drosy.Application.UseCases.Attendences.DTOs;
using Drosy.Application.UseCases.Attendences.Interfaces;
using Drosy.Domain.Entities;
using Drosy.Domain.Enums;
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


        [HttpGet("{studentId:int}", Name = "GetAttendenceById")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] int sessionId, [FromRoute] int studentId, CancellationToken ct)
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


        [HttpGet(Name = "GetSessionAttendences")]
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


        [HttpGet("student/{studentId:int}", Name = "GetSessionStudentAttendences")]
        public async Task<IActionResult> GetAllForStudentInSessionAsync([FromRoute] int sessionId, [FromRoute] int studentId, CancellationToken ct)
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


        [HttpGet("status", Name = "GetSessionAttendencesByStatus")]
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


        [HttpPost(Name = "AddStudentAttendencesForSession")]
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


        [HttpPost("batch", Name = "AddRangeOfStudentAttendenceForSession")]
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


        [HttpPut("{studentId:int}", Name = "UpdateAttendence")]
        public async Task<IActionResult> UpdateAsync([FromRoute] int sessionId, [FromRoute] int studentId, [FromBody] UpdateAttendencenDto dto, CancellationToken ct)
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


        [HttpDelete("{studentId:int}", Name = "DeleteAttendence")]
        public async Task<IActionResult> DeleteAsync( [FromRoute] int sessionId,   [FromRoute] int studentId,  CancellationToken ct) {
     
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
