
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
                    return BadRequest("Student data is required.");
                var result = await _studentService.AddAsync(dto);
                if (result.IsFailure)
                    return BadRequest(result.Error);
                return CreatedAtAction(nameof(GetByIdAsync), new { id = result.Value.Id }, result.Value);
            }
            catch (Exception)
            {
                // Log the exception (not implemented here)
                return StatusCode(500, Error.Invalid);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
           try
            {
                var result = await _studentService.GetByIdAsync(id);

                if (result.IsFailure && result.Error == Error.NotFound)
                    return NotFound(result.Error);

                return Ok(result.Value);
            }
            catch (Exception)
            {
                // Log the exception (not implemented here)
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}