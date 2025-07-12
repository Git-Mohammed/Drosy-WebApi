using Drosy.Api.Commons.Responses;
using Drosy.Application.UseCases.PlanStudents.DTOs;
using Drosy.Application.UseCases.PlanStudents.Interfaces;
using Drosy.Domain.Shared.ResultPattern.ErrorComponents;
using Microsoft.AspNetCore.Mvc;

namespace Drosy.Api.Controllers
{
    [Route("api/plans/{planId:int}/planstudents")]
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
                return ResponseHandler.SuccessResponse("Student retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ResponseHandler.HandleException(ex);
            }
        }
        #endregion
        #region Create
        [HttpPost(Name = "AddStudentToPlan")]
        public async Task<IActionResult> AddAsync(int planId, [FromBody] AddStudentToPlanDto dto, CancellationToken ct)
        {
            try
            {
                if (dto == null)
                {
                    var error = new ApiError("dto", ErrorMessagesRepository.GetMessage(Error.NullValue.Message, Error.CurrentLanguage));
                    return ResponseHandler.BadRequestResponse("dto", "Invalid student data.", error.Message);
                }

                var result = await _PlanStudentsService.AddStudentToPlanAsync(planId, dto, ct);

                if (result.IsFailure)
                {
                    return ResponseHandler.HandleFailure(result, nameof(AddAsync), "PlanStudent");
                }

                return ResponseHandler.CreatedResponse(
                   "GetPlanStudentById",
                    new { planId, id = result.Value.StudentId },
                    result.Value, "Student added to plan successfully."
                );
            }
            catch (Exception ex)
            {
                return ResponseHandler.HandleException(ex);
            }
        }

        [HttpPost("batch" ,Name = "AddRangeOfStudentToPlan")]
        public async Task<IActionResult> AddRangeAsync(int planId, [FromBody] IEnumerable<AddStudentToPlanDto> dtos, CancellationToken ct)
        {
            try
            {
                if (dtos == null || !dtos.Any())
                {
                    var err = new ApiError("dtos", ErrorMessagesRepository.GetMessage(Error.NullValue.Message, Error.CurrentLanguage));
                    return ResponseHandler.BadRequestResponse("dtos", err.Message, err.Message);
                }


                var result = await _PlanStudentsService.AddRangeOfStudentToPlanAsync(planId, dtos, ct);

                if (result.IsFailure)
                {
                    return ResponseHandler.HandleFailure(result, nameof(AddAsync), "PlanStudent");
                }

                // alter
                return ResponseHandler.CreatedResponse(
                   "GetPlanStudentById",
                   new {planId,studentId=1 }, "Student added to plan successfully."
                );
            }
            catch (Exception ex)
            {
                return ResponseHandler.HandleException(ex);
            }
        }

        #endregion
    }
}
