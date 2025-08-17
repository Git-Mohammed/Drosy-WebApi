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
                var error = new ApiError("id", "Invalid session ID.");
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


        //GET /api/sessions? date = 2023 - 10 - 15
        //GET /api/sessions? start = 2023 - 10 - 01 & end = 2023 - 10 - 07
        //GET /api/sessions? planStatus = Active
        //GET /api/sessions? year = 2023 & week = 42
        //GET /api/sessions? year = 2023 & month = 10
        //GET /api/sessions
        [HttpGet(Name = "GetSessions")]
        [ProducesResponseType(typeof(ApiResponse<DataResult<SessionDTO>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 422)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> GetListAsync(
            [FromQuery] DateTime? date,
            [FromQuery] DateTime? start,
            [FromQuery] DateTime? end,
            [FromQuery] SessionStatus? status,
            [FromQuery] int? year,
            [FromQuery] int? week,
            [FromQuery] int? month,
            CancellationToken ct)
        {
            // Guards
            if (week.HasValue && !year.HasValue)
                return ApiResponseFactory.BadRequestResponse("week", "`week` requires `year`.");
            if (month.HasValue && !year.HasValue)
                return ApiResponseFactory.BadRequestResponse("month", "`month` requires `year`.");
            if (week.HasValue && month.HasValue)
                return ApiResponseFactory.BadRequestResponse("filters", "Cannot filter by both `week` and `month`.");

            // Dispatch
            if (date.HasValue)
                return await Wrappers.WrapListFilter(
                    () => _sessionService.GetSessionsByDate(date.Value, ct),
                    $"Sessions on {date:yyyy-MM-dd}",
                    nameof(GetListAsync),
                    "Sessions");

            if (start.HasValue && end.HasValue)
                return await Wrappers.WrapListFilter(
                    () => _sessionService.GetSessionsInRange(start.Value, end.Value, ct),
                    $"Sessions from {start:yyyy-MM-dd} to {end:yyyy-MM-dd}",
                    nameof(GetListAsync),
                    "Sessions");

            if (status.HasValue)
                return await Wrappers.WrapListFilter(
                    () => _sessionService.GetSessionsByStatus(status.Value, ct),
                    $"Sessions with status {status}",
                    nameof(GetListAsync),
                    "Sessions");

            if (year.HasValue && week.HasValue)
                return await Wrappers.WrapListFilter(
                    () => _sessionService.GetSessionsByWeek(year.Value, week.Value, ct),
                    $"Sessions in ISO week {week}/{year}",
                    nameof(GetListAsync),
                    "Sessions");

            if (year.HasValue && month.HasValue)
                return await Wrappers.WrapListFilter(
                    () => _sessionService.GetSessionsByMonth(year.Value, month.Value, ct),
                    $"Sessions in {year}-{month:D2}",
                    nameof(GetListAsync),
                    "Sessions");

            // No filters: return all
            return await Wrappers.WrapListFilter(
                () => _sessionService.GetAllAsync(ct),
                "All sessions",
                nameof(GetListAsync),
                "Sessions");
        }


        //GET /api/sessions/calendar? date = 2023 - 10 - 15
        //GET /api/sessions/calendar? start = 2023 - 10 - 01 & end = 2023 - 10 - 07
        //GET /api/sessions/calendar? planStatus = Active
        //GET /api/sessions/calendar? year = 2023 & week = 42
        //GET /api/sessions/calendar? year = 2023 & month = 10
        //GET /api/sessions/calendar
        [HttpGet("calendar", Name = "GetSessionsCalendar")]
        [ProducesResponseType(typeof(ApiResponse<DataResult<CalenderSessionDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 422)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> GetSessionsCalendarAsync(
            [FromQuery] DateTime? date,
            [FromQuery] DateTime? start,
            [FromQuery] DateTime? end,
            [FromQuery] PlanStatus? planStatus,
            [FromQuery] int? year,
            [FromQuery] int? week,
            [FromQuery] int? month,
            CancellationToken ct)
        {
            // Validation guards
            if (week.HasValue && !year.HasValue)
                return ApiResponseFactory.BadRequestResponse("week", "`week` requires `year`.");
            if (month.HasValue && !year.HasValue)
                return ApiResponseFactory.BadRequestResponse("month", "`month` requires `year`.");
            if (week.HasValue && month.HasValue)
                return ApiResponseFactory.BadRequestResponse("filters", "Cannot filter by both `week` and `month`.");

            // Dispatch to the appropriate service method based on parameters
            if (date.HasValue)
                return await Wrappers.WrapListFilter(
                    () => _sessionService.GetSessionsCalenderByDate(date.Value, ct),
                    $"Calendar sessions on {date:yyyy-MM-dd}",
                    nameof(GetSessionsCalendarAsync),
                    "CalendarSessions");

            if (start.HasValue && end.HasValue)
                return await Wrappers.WrapListFilter(
                    () => _sessionService.GetSessionsCalenderInRange(start.Value, end.Value, ct),
                    $"Calendar sessions from {start:yyyy-MM-dd} to {end:yyyy-MM-dd}",
                    nameof(GetSessionsCalendarAsync),
                    "CalendarSessions");

            if (planStatus.HasValue)
                return await Wrappers.WrapListFilter(
                    () => _sessionService.GetSessionsCalenderByStatus(planStatus.Value, ct),
                    $"Calendar sessions with plan status {planStatus}",
                    nameof(GetSessionsCalendarAsync),
                    "CalendarSessions");

            if (year.HasValue && week.HasValue)
                return await Wrappers.WrapListFilter(
                    () => _sessionService.GetSessionsCalenderByWeek(year.Value, week.Value, ct),
                    $"Calendar sessions in ISO week {week}/{year}",
                    nameof(GetSessionsCalendarAsync),
                    "CalendarSessions");

            if (year.HasValue && month.HasValue)
                return await Wrappers.WrapListFilter(
                    () => _sessionService.GetSessionsCalenderByMonth(year.Value, month.Value, ct),
                    $"Calendar sessions in {year}-{month:D2}",
                    nameof(GetSessionsCalendarAsync),
                    "CalendarSessions");

            // Default case: no filters provided, return all calendar sessions
            return await Wrappers.WrapListFilter(
                () => _sessionService.GetSessionsCalenderAsync(ct),
                "All calendar sessions",
                nameof(GetSessionsCalendarAsync),
                "CalendarSessions");
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
            if (dto == null || string.IsNullOrEmpty(dto.Title))
            {
                var error = new ApiError("dto", "Invalid session data.");
                return ApiResponseFactory.UnprocessableEntityResponse("Session creation failed.");
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

                return ApiResponseFactory.SuccessResponse(result.Value, "Session rescheduled successfully.");

            }
            catch (Exception ex)
            {
                return ApiResponseFactory.FromException(ex);
            }
        }
        #endregion
    }
}
