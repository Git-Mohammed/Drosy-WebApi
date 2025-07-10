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

        [HttpPost]
        public async Task<IActionResult> AddAsync([FromBody] AddStudentDTO dto)
        {
            try
            {
                if (dto == null)
                {
                    var error = new ApiError("dto", ErrorMessagesRepository.GetMessage(Error.NullValue.Message, Error.CurrentLanguage));
                    return BadRequest(ApiResponse<StudentDTO>.Failure(new List<ApiError> { error }, "Invalid student data."));
                }

                var result = await _studentService.AddAsync(dto);
                if (result.IsFailure)
                {
                    var error = new ApiError("AddAsync", result.Error.Message);
                    return BadRequest(ApiResponse<StudentDTO>.Failure(new List<ApiError> { error }, "Failed to add student."));
                }

                return CreatedAtAction(
                    nameof(GetByIdAsync),
                    new { id = result.Value.Id },
                    ApiResponse<StudentDTO>.Success(result.Value, "Student added successfully.")
                );
            }
            catch (Exception)
            {
                var error = new ApiError("Exception", ErrorMessagesRepository.GetMessage("Error.Failure", Error.CurrentLanguage));
                return StatusCode(500, ApiResponse<StudentDTO>.Failure(new List<ApiError> { error }, "Internal server error."));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            try
            {
                var result = await _studentService.GetByIdAsync(id);

                if (result.IsFailure)
                {
                    var code = result.Error.Code == Error.NotFound.Code ? "Error.NotFound" : "Error.Failure";
                    var error = new ApiError("GetByIdAsync", ErrorMessagesRepository.GetMessage(code, Error.CurrentLanguage));
                    var status = result.Error == Error.NotFound ? 404 : 500;
                    return StatusCode(status, ApiResponse<StudentDTO>.Failure(new List<ApiError> { error }, "Could not retrieve student."));
                }

                return Ok(ApiResponse<StudentDTO>.Success(result.Value, "Student retrieved successfully."));
            }
            catch (Exception)
            {
                var error = new ApiError("Exception", ErrorMessagesRepository.GetMessage("Error.Failure", Error.CurrentLanguage));
                return StatusCode(500, ApiResponse<StudentDTO>.Failure(new List<ApiError> { error }, "Internal server error."));
            }
        }
    }
}
