using Drosy.Api.Commons.Responses;
using Drosy.Application.UseCases.SystemSettings.DTOs;
using Drosy.Application.UseCases.SystemSettings.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Drosy.Presentation.API.Controllers
{
    /// <summary>
    /// Controller for managing system-wide configuration settings such as web name, default currency, and logo.
    /// </summary>
    [ApiController]
    [Route("api/system-settings")]
    public class SystemSettingsController : ControllerBase
    {
        private readonly ISystemSettingService _systemSettingService;
        private readonly ILogger<SystemSettingsController> _logger;


        /// <summary>
        /// Initializes a new instance of the <see cref="SystemSettingsController"/> class.
        /// </summary>
        /// <param name="systemSettingService">Service for handling system setting operations.</param>
        /// <param name="logger">Logger instance for logging errors and diagnostics.</param>
        public SystemSettingsController(
            ISystemSettingService systemSettingService,
            ILogger<SystemSettingsController> logger)
        {
            _systemSettingService = systemSettingService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves the current system settings including web name, default currency, and logo path.
        /// </summary>
        /// <param name="ct">Cancellation token for the request.</param>
        /// <returns>
        /// A <see cref="SystemSettingDTO"/> wrapped in an <see cref="ApiResponse{T}"/> if successful;
        /// otherwise, an error response indicating failure or not found.
        /// </returns>
        [HttpGet(Name = "GetSystemSettings")]
        [ProducesResponseType(typeof(ApiResponse<SystemSettingDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAsync(CancellationToken ct)
        {
            try
            {
                var result = await _systemSettingService.GetAsync(ct);
                if (result.IsFailure)
                    return ApiResponseFactory.FromFailure(result, nameof(GetAsync), "SystemSetting");

                return ApiResponseFactory.SuccessResponse(result.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving system settings.");
                return ApiResponseFactory.FromException(ex);
            }
        }

        /// <summary>
        /// Updates the system settings including web name, default currency, and optionally the logo file.
        /// </summary>
        /// <param name="dto">DTO containing updated values for system settings.</param>
        /// <param name="ct">Cancellation token for the request.</param>
        /// <returns>
        /// A <see cref="SystemSettingDTO"/> wrapped in an <see cref="ApiResponse{T}"/> if update is successful;
        /// otherwise, an error response indicating validation failure or unexpected error.
        /// </returns>
        [HttpPut(Name = "UpdateSystemSettings")]
        [ProducesResponseType(typeof(ApiResponse<SystemSettingDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateAsync([FromForm] UpdateSystemSettingDTO dto, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(dto.WebName))
            {
                var error = new ApiError(nameof(dto.WebName), "Web name is required.");
                return ApiResponseFactory.BadRequestResponse(nameof(dto.WebName), error.Message, error.Message);
            }

            if (string.IsNullOrWhiteSpace(dto.DefaultCurrency))
            {
                var error = new ApiError(nameof(dto.DefaultCurrency), "Default currency is required.");
                return ApiResponseFactory.BadRequestResponse(nameof(dto.DefaultCurrency), error.Message, error.Message);
            }

            try
            {
                var result = await _systemSettingService.UpdateAsync(dto, ct);
                if (result.IsFailure)
                    return ApiResponseFactory.FromFailure(result, nameof(UpdateAsync), "SystemSetting");

                return ApiResponseFactory.SuccessResponse(result.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating system settings.");
                return ApiResponseFactory.FromException(ex);
            }
        }
    }
}
