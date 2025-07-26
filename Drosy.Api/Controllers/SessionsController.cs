using Drosy.Api.Commons.Responses;
using Drosy.Application.UseCases.Sessions.DTOs;
using Drosy.Application.UseCases.Sessions.Interfaces;
using Drosy.Domain.Enums;
using Drosy.Domain.Shared.ApplicationResults;
using Drosy.Domain.Shared.ErrorComponents;
using Drosy.Domain.Shared.ErrorComponents.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Drosy.Api.Controllers
{
    [ApiController]
    [Route("api/sessions")]
    [Authorize]
    public class SessionsController : ControllerBase
    {
        private readonly ISessionService _sessionService;

        public SessionsController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        #region 🔍 Read

        /// <summary>
        /// Retrieves session details by its ID.
        /// </summary>
        /// <param name="id">The unique session identifier.</param>
        /// <param name="ct">A cancellation token.</param>
        /// <returns>
        /// 200 OK if the session is found.  
        /// 400 Bad Request if <paramref name="id"/> is less than 1.  
        /// 404 Not Found if the session does not exist.  
        /// 422 Unprocessable Entity for domain-related validation errors.  
        /// 500 Internal Server Error for unexpected exceptions.
        /// </returns>
        [HttpGet("{id:int}", Name = "GetSessionByIdAsync")]
        [ProducesResponseType(typeof(ApiResponse<SessionDTO>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 422)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> GetByIdAsync(int id, CancellationToken ct)
        {
            if (id < 1)
            {
                var error = new ApiError("id", ErrorMessageResourceRepository.GetMessage(CommonErrors.Invalid.Message, AppError.CurrentLanguage));
                return ApiResponseFactory.BadRequestResponse("id", "Invalid session ID.", error.Message);
            }

            try
            {
                var result = await _sessionService.GetByIdAsync(id, ct);

                if (result.IsFailure)
                {
                    return ApiResponseFactory.FromFailure(result, nameof(GetByIdAsync));
                }

                return ApiResponseFactory.SuccessResponse(result.Value, "Session retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponseFactory.FromException(ex);
            }
        }

        /// <summary>
        /// Retrieves all sessions scheduled on a specific date.
        /// </summary>
        /// <param name="date">The date to search sessions for.</param>
        /// <param name="ct">A cancellation token.</param>
        /// <returns>
        /// 200 OK with the list of sessions.  
        /// 400 Bad Request for invalid input.  
        /// 422 Unprocessable Entity for domain errors.  
        /// 500 Internal Server Error for unexpected issues.
        /// </returns>
        [HttpGet("by-date", Name = "GetSessionsByDateAsync")]
        [ProducesResponseType(typeof(ApiResponse<DataResult<SessionDTO>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 422)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> GetSessionsByDateAsync(
            [FromQuery] DateTime date,
            CancellationToken ct)
        {
            try
            {
                var result = await _sessionService.GetSessionsByDate( date, ct);
                if (result.IsFailure)
                    return ApiResponseFactory.FromFailure(result, nameof(GetSessionsByDateAsync));

                return ApiResponseFactory.SuccessResponse(result.Value, "Sessions for date retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponseFactory.FromException(ex);
            }
        }


        /// <summary>
        /// Retrieves sessions that fall within the specified date range.
        /// </summary>
        /// <param name="start">Start of the date range.</param>
        /// <param name="end">End of the date range.</param>
        /// <param name="ct">A cancellation token.</param>
        /// <returns>
        /// 200 OK with the list of sessions.  
        /// 400 Bad Request for invalid date range.  
        /// 422 Unprocessable Entity for domain issues.  
        /// 500 Internal Server Error for unexpected problems.
        /// </returns>
        [HttpGet("by-date-range", Name = "GetSessionsInRangeAsync")]
        [ProducesResponseType(typeof(ApiResponse<DataResult<SessionDTO>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 422)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> GetSessionsInRangeAsync(
        [FromQuery] DateTime start,
        [FromQuery] DateTime end,
        CancellationToken ct)
        {
        
            try
            {
                var result = await _sessionService.GetSessionsInRange( start, end, ct);
                if (result.IsFailure)
                    return ApiResponseFactory.FromFailure(result, nameof(GetSessionsInRangeAsync));

                return ApiResponseFactory.SuccessResponse(result.Value, "Sessions in range retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponseFactory.FromException(ex);
            }
        }

        /// <summary>
        /// Retrieves sessions for a given plan in a specific calendar week.
        /// </summary>
        /// <param name="planId">The plan ID.</param>
        /// <param name="year">The calendar year.</param>
        /// <param name="weekNumber">The ISO 8601 week number.</param>
        /// <param name="ct">A cancellation token.</param>
        /// <returns>
        /// 200 OK with the sessions for the specified week.  
        /// 400 Bad Request for invalid input.  
        /// 422 Unprocessable Entity for business rule violations.  
        /// 500 Internal Server Error for unexpected failures.
        /// </returns>
        [HttpGet("week/{planId:int}", Name = "GetSessionsByWeekAsync")]
        [ProducesResponseType(typeof(ApiResponse<DataResult<SessionDTO>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 422)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> GetSessionsByWeekAsync(
           int planId,
           [FromQuery] int year,
           [FromQuery] int weekNumber,
           CancellationToken ct)
        {
         
            try
            {
                var result = await _sessionService.GetSessionsByWeek( year, weekNumber, ct);
                if (result.IsFailure)
                    return ApiResponseFactory.FromFailure(result, nameof(GetSessionsByWeekAsync));

                return ApiResponseFactory.SuccessResponse(result.Value, "Sessions by week retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponseFactory.FromException(ex);
            }
        }

        /// <summary>
        /// Retrieves sessions for a specific plan in a given month and year.
        /// </summary>
        /// <param name="planId">The plan ID.</param>
        /// <param name="year">The year to filter by.</param>
        /// <param name="month">The month (1-12).</param>
        /// <param name="ct">A cancellation token.</param>
        /// <returns>
        /// 200 OK with the sessions.  
        /// 400 Bad Request for invalid parameters.  
        /// 422 Unprocessable Entity for domain errors.  
        /// 500 Internal Server Error for unexpected conditions.
        /// </returns>
        [HttpGet("month/{planId:int}", Name = "GetSessionsByMonthAsync")]
        [ProducesResponseType(typeof(ApiResponse<DataResult<SessionDTO>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 422)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> GetSessionsByMonthAsync(
         int planId,
         [FromQuery] int year,
         [FromQuery] int month,
         CancellationToken ct)
        {
       
            try
            {
                var result = await _sessionService.GetSessionsByMonth(year, month, ct);
                if (result.IsFailure)
                    return ApiResponseFactory.FromFailure(result, nameof(GetSessionsByMonthAsync));

                return ApiResponseFactory.SuccessResponse(result.Value, "Sessions by month retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponseFactory.FromException(ex);
            }
        }

        /// <summary>
        /// Retrieves sessions by their status under a specific plan.
        /// </summary>
        /// <param name="planId">The plan ID.</param>
        /// <param name="status">The session status to filter by.</param>
        /// <param name="ct">A cancellation token.</param>
        /// <returns>
        /// 200 OK with matching sessions.  
        /// 400 Bad Request for invalid status.  
        /// 422 Unprocessable Entity for business logic issues.  
        /// 500 Internal Server Error for system errors.
        /// </returns>
        [HttpGet("status/{planId:int}", Name = "GetSessionsByStatusAsync")]
        [ProducesResponseType(typeof(ApiResponse<DataResult<SessionDTO>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 422)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> GetSessionsByStatusAsync(
            [FromQuery] SessionStatus status,
            CancellationToken ct)
        {
            try
            {
                var result = await _sessionService.GetSessionsByStatus(status, ct);
                if (result.IsFailure)
                    return ApiResponseFactory.FromFailure(result, nameof(GetSessionsByStatusAsync));

                return ApiResponseFactory.SuccessResponse(result.Value, "Sessions by status retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponseFactory.FromException(ex);
            }
        }

        #endregion

        #region 🆕 Create

        /// <summary>
        /// Adds a new session to the system.
        /// </summary>
        /// <param name="dto">Session data transfer object.</param>
        /// <param name="ct">A cancellation token.</param>
        /// <returns>
        /// 201 Created on successful session addition.  
        /// 400 Bad Request if <paramref name="dto"/> is null or invalid.  
        /// 422 Unprocessable Entity for domain validation failures.  
        /// 500 Internal Server Error on unexpected exceptions.
        /// </returns>
        [HttpPost(Name = "CreateSessionAsync")]
        [ProducesResponseType(typeof(ApiResponse<SessionDTO>), 201)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 422)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> CreateAsync([FromBody] CreateSessionDTO dto, CancellationToken ct)
        {
            if (dto == null)
            {
                var error = new ApiError("dto", ErrorMessageResourceRepository.GetMessage(CommonErrors.NullValue.Message, AppError.CurrentLanguage));
                return ApiResponseFactory.BadRequestResponse("dto", "Invalid session data.", error.Message);
            }

            try
            {
                var result = await _sessionService.CreateAsync(dto, ct);

                if (result.IsFailure)
                {
                    return ApiResponseFactory.FromFailure(result, nameof(CreateAsync), "Session");
                }

                return ApiResponseFactory.CreatedResponse(
                    "GetSessionByIdAsync",
                    new { id = result.Value.Id },
                    result.Value,
                    "Session added successfully."
                );
            }
            catch (Exception ex)
            {
                return ApiResponseFactory.FromException(ex);
            }
        }


        /// <summary>
        /// Reschedules an existing session.
        /// </summary>
        /// <param name="sessionId">The identifier of the session to update.</param>
        /// <param name="dto">Reschedule session data transfer object.</param>
        /// <param name="ct">A cancellation token.</param>
        /// <returns>
        /// 200 OK on successful rescheduling.  
        /// 400 Bad Request if <paramref name="sessionId"/> is less than 1 or input DTO is invalid.  
        /// 404 Not Found if the session does not exist.  
        /// 422 Unprocessable Entity for domain validation failures.  
        /// 500 Internal Server Error on exceptions.
        /// </returns>
        [HttpPut("{sessionId:int}", Name = "RescheduleSessionAsync")]
        [ProducesResponseType(typeof(ApiResponse<SessionDTO>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 422)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> RescheduleAsync(int sessionId, [FromBody] RescheduleSessionDTO dto, CancellationToken ct)
        {
            if (sessionId < 1)
            {
                var error = new ApiError("sessionId", ErrorMessageResourceRepository.GetMessage(CommonErrors.Invalid.Message, AppError.CurrentLanguage));
                return ApiResponseFactory.BadRequestResponse("sessionId", "Invalid session ID.", error.Message);
            }

            if (dto == null)
            {
                var error = new ApiError("dto", ErrorMessageResourceRepository.GetMessage(CommonErrors.NullValue.Message, AppError.CurrentLanguage));
                return ApiResponseFactory.BadRequestResponse("dto", "Invalid reschedule data.", error.Message);
            }

            try
            {
                var result = await _sessionService.RescheduleAsync(sessionId, dto, ct);

                if (result.IsFailure)
                {
                    return ApiResponseFactory.FromFailure(result, nameof(RescheduleAsync), "Session");
                }

                return ApiResponseFactory.SuccessResponse(
                    result.Value,
                    "Session rescheduled successfully."
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
