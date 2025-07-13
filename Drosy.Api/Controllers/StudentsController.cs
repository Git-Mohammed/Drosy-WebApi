using Drosy.Api.Commons.Responses;
using Drosy.Application.UseCases.Students.DTOs;
using Drosy.Application.UseCases.Students.Interfaces;
using Drosy.Domain.Shared.ResultPattern.ErrorComponents;
using Microsoft.AspNetCore.Mvc;

namespace Drosy.Api.Controllers
{
    // TODO:
    // - Athourization
    // - add canclelation token to all async methods

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

        /// <summary>
        /// Retrieves the student entity with the specified ID.
        /// </summary>
        /// <param name="id">The unique identifier of the student.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the student details if found; 
        /// otherwise, an appropriate error response.
        /// </returns>
        [HttpGet("{id:int}", Name = "GetStudentByIdAsync")]
        public async Task<IActionResult> GetByIdAsync(int id, CancellationToken  cancellationToken)
        {
            try
            {
                var result = await _studentService.GetByIdAsync(id, cancellationToken);

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

        #region Write
        /// <summary>
        /// Adds a new student using the provided data transfer object.
        /// </summary>
        /// <param name="dto">The student data to be added.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating success with the new student's ID or failure details.
        /// </returns>
        [HttpPost(Name = "AddStudentAsync")]
        public async Task<IActionResult> AddAsync([FromBody] AddStudentDTO dto, CancellationToken cancellationToken)
        {
            try
            {
                if (dto == null)
                {
                    var error = new ApiError("dto", ErrorMessagesRepository.GetMessage(Error.NullValue.Message, Error.CurrentLanguage));
                    return ResponseHandler.BadRequestResponse("dto", "Invalid student data.", error.Message);
                }
                
                var result = await _studentService.AddAsync(dto, cancellationToken);

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

        /// <summary>
        /// Updates an existing student with the provided data transfer object and ID.
        /// </summary>
        /// <param name="dto">The updated student data.</param>
        /// <param name="id">The unique identifier of the student to be updated.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the update operation.
        /// </returns>
        [HttpPut("{id:int}",Name = "UpdateStudentAsync")]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateStudentDTO dto, int id, CancellationToken cancellationToken)
        {
            if(id <= 0)
            {
                var error = new ApiError("id", ErrorMessagesRepository.GetMessage(Error.Invalid.Message, Error.CurrentLanguage));
                return ResponseHandler.BadRequestResponse("id", "Invalid student ID.", error.Message);
            }

            if (dto == null)
            {
                var error = new ApiError("dto", ErrorMessagesRepository.GetMessage(Error.NullValue.Message, Error.CurrentLanguage));
                return ResponseHandler.BadRequestResponse("dto", "Invalid student data.", error.Message);
            }

            try
            {

                var result = await _studentService.UpdateAsync(dto, id, cancellationToken);

                if (result.IsFailure)
                {
                    return ResponseHandler.HandleFailure(result, nameof(UpdateAsync), "Student");
                }

                return ResponseHandler.SuccessResponse( "Student updated successfully.");
            }
            catch (Exception ex)
            {
                return ResponseHandler.HandleException(ex);
            }
        }

        #endregion
    }
}
