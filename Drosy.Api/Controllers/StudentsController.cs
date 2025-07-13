using Drosy.Api.Commons.Responses;
using Drosy.Application.UseCases.Students.DTOs;
using Drosy.Application.UseCases.Students.Interfaces;
using Drosy.Domain.Shared.ResultPattern.ErrorComponents;
using Microsoft.AspNetCore.Mvc;

namespace Drosy.Api.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;
        public StudentsController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        #region Read
        [HttpGet("{id:int}", Name = "GetStudentByIdAsync")]
        public async Task<IActionResult> GetByIdAsync(int id, CancellationToken ct)
        {
            try
            {
                var result = await _studentService.GetByIdAsync(id, ct);

                if (result.IsFailure)
                {
                    return ResponseHandler.HandleFailure(result, nameof(GetByIdAsync));
                }

                return ResponseHandler.SuccessResponse(result.Value, "Student retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ResponseHandler.HandleException(ex);
            }
        }
        #endregion

        #region Create
        [HttpPost(Name = "AddStudentAsync")]
        public async Task<IActionResult> AddAsync([FromBody] AddStudentDTO dto, CancellationToken ct)
        {
            try
            {
                if (dto == null)
                {
                    var error = new ApiError("dto", ErrorMessagesRepository.GetMessage(Error.NullValue.Message, Error.CurrentLanguage));
                    return ResponseHandler.BadRequestResponse("dto", "Invalid student data.", error.Message);
                }
                
                var result = await _studentService.AddAsync(dto,ct);

                if (result.IsFailure)
                {
                    return ResponseHandler.HandleFailure(result, nameof(AddAsync), "Student");
                }

                return ResponseHandler.CreatedResponse(
                   "GetStudentByIdAsync",
                    new { id = result.Value.Id },
                    result.Value, "Student added successfully."
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
