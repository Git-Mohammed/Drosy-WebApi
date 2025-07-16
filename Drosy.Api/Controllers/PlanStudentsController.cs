using Drosy.Api.Commons.Responses;
using Drosy.Application.UseCases.PlanStudents.DTOs;
using Drosy.Application.UseCases.PlanStudents.Interfaces;
using Drosy.Application.UseCases.PlanStudents.Services;
using Drosy.Domain.Shared.DataDTOs;
using Drosy.Domain.Shared.ErrorComponents;
using Drosy.Domain.Shared.ResultPattern.ErrorComponents.Common;
using Microsoft.AspNetCore.Mvc;

namespace Drosy.Api.Controllers
{
    [Route("api/plans/{planId:int}/students")]
    [ApiController]
    public class PlanStudentsController : ControllerBase
    {
        private readonly IPlanStudentsService _PlanStudentsService;
        public PlanStudentsController(IPlanStudentsService PlanStudentsService)
        {
            _PlanStudentsService = PlanStudentsService;
        }

        #region Read
        [HttpGet("{studentId:int}", Name = "GetPlanStudentById")]
        public async Task<IActionResult> GetByIdAsync(int planId, int studentId, CancellationToken ct)
        {
            try
            {
                // alter
                return ApiResponseFactory.SuccessResponse("Student retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponseFactory.FromException(ex);
            }
        }

        [HttpGet(Name = "GetPlanStudents")]
        public async Task<IActionResult> GetAllAsync(int planId, CancellationToken ct)
        {
            //var result = await _planStudentsService.GetStudentsInPlanAsync(planId, ct);
            //if (result.IsFailure)
            //    return ResponseHandler.HandleFailure(result, nameof(GetAllAsync), "PlanStudent");

            return ApiResponseFactory.SuccessResponse(new { }, "Students retrieved successfully.");
        }

        #endregion

        #region Create
        /// <summary>
        /// Adds a single student to the specified plan.
        /// </summary>
        /// <param name="planId">The ID of the plan.</param>
        /// <param name="dto">The student data.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>
        /// 201 (Created) with the created student;  
        /// 400 (Bad Request) if <paramref name="dto"/> is null;  
        /// 404 (Not Found) if the student (or plan) does not exist;  
        /// 409 (Conflict) if the student is already in the plan;  
        /// 422 (Unprocessable Entity) for validation failures;  
        /// 500 (Internal Server Error) on unexpected errors.
        /// </returns>
        [HttpPost(Name = "AddStudentToPlan")]
        [ProducesResponseType(typeof(ApiResponse<PlanStudentDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddAsync(int planId, [FromBody] AddStudentToPlanDto dto, CancellationToken ct)
        {
            try
            {
                if (dto == null)
                {
                    var error = new ApiError("dto", ErrorMessageResourceRepository.GetMessage(CommonErrors.NullValue.Message, AppError.CurrentLanguage));
                    return ApiResponseFactory.BadRequestResponse("dto", "Invalid student data.", error.Message);
                }

                var result = await _PlanStudentsService.AddStudentToPlanAsync(planId, dto, ct);

                if (result.IsFailure)
                {
                    return ApiResponseFactory.FromFailure(result, nameof(AddAsync), "PlanStudent");
                }

                return ApiResponseFactory.CreatedResponse(
                   "GetPlanStudentById",
                    new { planId, id = result.Value.StudentId },
                    result.Value, "Student added to plan successfully."
                );
            }
            catch (Exception ex)
            {
                return ApiResponseFactory.FromException(ex);
            }
        }


        /// <summary>
        /// Adds multiple students to the specified plan.
        /// </summary>
        /// <param name="planId">The ID of the plan.</param>
        /// <param name="dtos">The list of student data.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>
        /// 201 (Created) with the list of created students;  
        /// 400 (Bad Request) if <paramref name="dtos"/> is null or empty;  
        /// 404 (Not Found) if any student (or plan) does not exist;  
        /// 409 (Conflict) if all students are already in the plan;  
        /// 500 (Internal Server Error) on unexpected errors.
        /// </returns>
        [HttpPost("batch", Name = "AddRangeOfStudentToPlan")]
        [ProducesResponseType(typeof(ApiResponse<DataResult<PlanStudentDto>>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddRangeAsync(int planId, [FromBody] IEnumerable<AddStudentToPlanDto> dtos, CancellationToken ct)
        {
            try
            {
                if (dtos == null || !dtos.Any())
                {
                    var err = new ApiError("dtos", ErrorMessageResourceRepository.GetMessage(CommonErrors.NullValue.Message, AppError.CurrentLanguage));
                    return ApiResponseFactory.BadRequestResponse("dtos", err.Message, err.Message);
                }


                var result = await _PlanStudentsService.AddRangeOfStudentToPlanAsync(planId, dtos, ct);

                if (result.IsFailure)
                {
                    return ApiResponseFactory.FromFailure(result, nameof(AddAsync), "PlanStudent");
                }

                return ApiResponseFactory.CreatedResponse(
                   "GetPlanStudents",
                   new { planId },
                   result.Value, "Student added to plan successfully."
                );
            }
            catch (Exception ex)
            {
                return ApiResponseFactory.FromException(ex);
            }
        }

        #endregion
    }
}
