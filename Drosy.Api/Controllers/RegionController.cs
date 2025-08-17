using Drosy.Application.UseCases.Regions.DTOs;
using Drosy.Application.UseCases.Regions.Interfaces;
using Drosy.Api.Commons.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Drosy.Api.Controllers;

[ApiController]
[Route("api/regions")]
public class RegionController(IRegionService regionService, ILogger<RegionController> logger) : ControllerBase
{
    private readonly IRegionService _regionService = regionService;
    private readonly ILogger<RegionController> _logger = logger;

    [HttpGet(Name = "GetAllRegionsAsync")]
    public async Task<IActionResult> GetAllAsync(CancellationToken ct)
    {
        try
        {
            var result = await _regionService.GetAllAsync(ct);

            if (result.IsFailure)
                return ApiResponseFactory.FromFailure(result, nameof(GetAllAsync), "Region");

            return ApiResponseFactory.SuccessResponse(result.Value, "Regions retrieved successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in GetAllAsync");
            return ApiResponseFactory.FromException(ex);
        }
    }

    [HttpGet("{id:int}", Name = "GetRegionByIdAsync")]
    public async Task<IActionResult> GetByIdAsync(int id, CancellationToken ct)
    {
        try
        {
            if (id < 1)
                return ApiResponseFactory.BadRequestResponse("id", "Invalid region ID.");

            var result = await _regionService.GetByIdAsync(id, ct);

            if (result.IsFailure)
                return ApiResponseFactory.FromFailure(result, nameof(GetByIdAsync), "Region");

            return ApiResponseFactory.SuccessResponse(result.Value, "Region retrieved successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in GetByIdAsync");
            return ApiResponseFactory.FromException(ex);
        }
    }

    [HttpPost(Name = "CreateRegionAsync")]
    public async Task<IActionResult> CreateAsync([FromBody] CreateRegionDTO dto, CancellationToken ct)
    {
        try
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Name))
                return ApiResponseFactory.UnprocessableEntityResponse("Region creation failed.");

            var result = await _regionService.CreateAsync(dto, ct);

            if (result.IsFailure)
                return ApiResponseFactory.FromFailure(result, nameof(CreateAsync), "Region");

            return ApiResponseFactory.CreatedResponse(
                "GetRegionByIdAsync",
                new { id = result.Value.Id },
                result.Value,
                "Region created successfully."
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in CreateAsync");
            return ApiResponseFactory.FromException(ex);
        }
    }

    [HttpPut("{id:int}", Name = "UpdateRegionAsync")]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] UpdateRegionDTO dto, CancellationToken ct)
    {
        try
        {
            if (id < 1)
                return ApiResponseFactory.BadRequestResponse("id", "Invalid region ID.");

            if (dto == null)
                return ApiResponseFactory.BadRequestResponse("dto", "Invalid update data.");

            var result = await _regionService.UpdateAsync(dto, id, ct);

            if (result.IsFailure)
                return ApiResponseFactory.FromFailure(result, nameof(UpdateAsync), "Region");

            return ApiResponseFactory.SuccessResponse("Region updated successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in UpdateAsync");
            return ApiResponseFactory.FromException(ex);
        }
    }

    [HttpDelete("{id:int}", Name = "DeleteRegionAsync")]
    public async Task<IActionResult> DeleteAsync(int id, CancellationToken ct)
    {
        try
        {
            if (id < 1)
                return ApiResponseFactory.BadRequestResponse("id", "Invalid region ID.");

            var result = await _regionService.DeleteAsync(id, ct);

            if (result.IsFailure)
                return ApiResponseFactory.FromFailure(result, nameof(DeleteAsync), "Region");

            return ApiResponseFactory.NoContentResponse("Region deleted successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in DeleteAsync");
            return ApiResponseFactory.FromException(ex);
        }
    }
}
