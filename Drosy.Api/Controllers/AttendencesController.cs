using Drosy.Api.Commons.Responses;
using Drosy.Application.UseCases.Attendences.DTOs;
using Drosy.Application.UseCases.Attendences.Interfaces;
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


        [HttpGet("{id:int}", Name = "GetPlanStudentById")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] int sessionId, [FromQuery] int id, CancellationToken ct)
        {
            try
            {
                return Ok();
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
                return Ok();
            }
            catch (Exception ex)
            {
                return ApiResponseFactory.FromException(ex);
            }
        }


        [HttpPost(Name = "AddStudentAttendenceForSession")]
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
                   "GetPlanStudentById",
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


        [HttpPut("{id:int}", Name = "AddRangeOfStudentAttendenceForSession")]
        public async Task<IActionResult> UpdateAsync([FromRoute] int sessionId, [FromQuery] int id, [FromBody] UpdateAttendencenDto dto, CancellationToken ct)
        {
            if (id < 1)
            {
                var error = new ApiError("id", ErrorMessageResourceRepository.GetMessage(CommonErrors.Invalid.Message, AppError.CurrentLanguage));
                return ApiResponseFactory.BadRequestResponse("id", "Invalid student ID.", error.Message);
            }

            if (dto == null)
            {
                var error = new ApiError("dto", ErrorMessageResourceRepository.GetMessage(CommonErrors.NullValue.Message, AppError.CurrentLanguage));
                return ApiResponseFactory.BadRequestResponse("dto", "Invalid student attendence data.", error.Message);
            }

            try
            {
                var result = await _attendencesService.UpdateAsync(sessionId, id,dto, ct);

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
    }
}
