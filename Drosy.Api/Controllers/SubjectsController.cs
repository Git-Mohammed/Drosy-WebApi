using Drosy.Api.Commons.Responses;
using Drosy.Application.UseCases.Subjects.DTOs;
using Drosy.Application.UseCases.Subjects.Interfaces;
using Drosy.Domain.Shared.ApplicationResults;
using Microsoft.AspNetCore.Mvc;

namespace Drosy.Api.Controllers;

/// <summary>
/// Manages subject-related operations including creation, retrieval, update, and deletion.
/// </summary>
[ApiController]
[Route("api/subjects")]
public class SubjectsController(ISubjectService subjectService, ILogger<SubjectsController> logger) : ControllerBase
{
    private readonly ISubjectService _subjectService = subjectService;
    private readonly ILogger<SubjectsController> _logger = logger;

    #region 📚 Read

    /// <summary>
    /// Retrieves all subjects.
    /// </summary>
    [HttpGet(Name = "GetAllSubjectsAsync")]
    [ProducesResponseType(typeof(ApiResponse<DataResult<SubjectDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllAsync(CancellationToken ct)
    {
        try
        {
            var result = await _subjectService.GetAllAsync(ct);
            if (result.IsFailure)
                return ApiResponseFactory.FromFailure(result, nameof(GetAllAsync), "Subject");

            return ApiResponseFactory.SuccessResponse(result.Value, "Subjects retrieved successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving subjects.");
            return ApiResponseFactory.FromException(ex);
        }
    }

    /// <summary>
    /// Retrieves a subject by its ID.
    /// </summary>
    [HttpGet("{id:int}", Name = "GetSubjectByIdAsync")]
    [ProducesResponseType(typeof(ApiResponse<SubjectDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByIdAsync(int id, CancellationToken ct)
    {
        if (id < 1)
        {
            var error = new ApiError("id", "Invalid subject ID.");
            return ApiResponseFactory.BadRequestResponse("id", "Invalid subject ID.", error.Message);
        }

        try
        {
            var result = await _subjectService.GetByIdAsync(id, ct);
            if (result.IsFailure)
                return ApiResponseFactory.FromFailure(result, nameof(GetByIdAsync), "Subject");

            return ApiResponseFactory.SuccessResponse(result.Value, "Subject retrieved successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving subject by ID.");
            return ApiResponseFactory.FromException(ex);
        }
    }

    #endregion

    #region 🆕 Create

    /// <summary>
    /// Creates a new subject.
    /// </summary>
    [HttpPost(Name = "CreateSubjectAsync")]
    [ProducesResponseType(typeof(ApiResponse<SubjectDTO>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateSubjectDTO dto, CancellationToken ct)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.Name))
        {
            var error = new ApiError("dto", "Invalid subject data.");
            return ApiResponseFactory.BadRequestResponse("dto", "Invalid subject data.", error.Message);
        }

        try
        {
            var result = await _subjectService.CreateAsync(dto, ct);
            if (result.IsFailure)
                return ApiResponseFactory.FromFailure(result, nameof(CreateAsync), "Subject");

            return ApiResponseFactory.CreatedResponse(
                "GetSubjectByIdAsync",
                new { id = result.Value.Id },
                result.Value,
                "Subject created successfully."
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating subject.");
            return ApiResponseFactory.FromException(ex);
        }
    }

    #endregion

    #region ✏️ Update

    /// <summary>
    /// Updates an existing subject.
    /// </summary>
    [HttpPut("{id:int}", Name = "UpdateSubjectAsync")]
    [ProducesResponseType(typeof(ApiResponse<SubjectDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] UpdateSubjectDTO dto, CancellationToken ct)
    {
        if (id < 1)
        {
            var error = new ApiError("id", "Invalid subject ID.");
            return ApiResponseFactory.BadRequestResponse("id", "Invalid subject ID.", error.Message);
        }

        if (dto == null)
        {
            var error = new ApiError("dto", "Invalid update data.");
            return ApiResponseFactory.BadRequestResponse("dto", "Invalid update data.", error.Message);
        }

        try
        {
            var result = await _subjectService.UpdateAsync(dto, id, ct);
            if (result.IsFailure)
                return ApiResponseFactory.FromFailure(result, nameof(UpdateAsync), "Subject");

            return ApiResponseFactory.SuccessResponse( "Subject updated successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating subject.");
            return ApiResponseFactory.FromException(ex);
        }
    }

    #endregion

    #region ❌ Delete

    /// <summary>
    /// Deletes a subject by its ID.
    /// </summary>
    [HttpDelete("{id:int}", Name = "DeleteSubjectAsync")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteAsync(int id, CancellationToken ct)
    {
        if (id < 1)
        {
            var error = new ApiError("id", "Invalid subject ID.");
            return ApiResponseFactory.BadRequestResponse("id", "Invalid subject ID.", error.Message);
        }

        try
        {
            var result = await _subjectService.DeleteAsync(id, ct);
            if (result.IsFailure)
                return ApiResponseFactory.FromFailure(result, nameof(DeleteAsync), "Subject");

            return ApiResponseFactory.NoContentResponse("Subject deleted successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting subject.");
            return ApiResponseFactory.FromException(ex);
        }
    }

    #endregion
}
