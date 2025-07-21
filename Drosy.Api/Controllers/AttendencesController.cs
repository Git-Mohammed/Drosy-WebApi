using Drosy.Api.Commons.Responses;
using Drosy.Application.UseCases.Attendences.DTOs;
using Drosy.Application.UseCases.Attendences.Interfaces;
using Drosy.Domain.Shared.ErrorComponents;
using Drosy.Domain.Shared.ErrorComponents.Common;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> GetByIdAsync(int sessionId, int id, CancellationToken ct)
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
        public async Task<IActionResult> GetAllForSessionAsync(int sessionId, CancellationToken ct)
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
        public async Task<IActionResult> AddAsync(int sessionId, [FromBody] AddAttendencenDto dto, CancellationToken ct)
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
        public async Task<IActionResult> AddRangeAsync(int sessionId, [FromBody] IEnumerable<AddAttendencenDto> dtos, CancellationToken ct)
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
    }
}
