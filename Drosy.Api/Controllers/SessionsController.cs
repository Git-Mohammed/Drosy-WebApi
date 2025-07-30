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

                        
        // GET /api/sessions?date=2025-07-26
        // GET /api/sessions?start=2025-07-01&end=2025-07-31
        // GET /api/sessions?status=Planned
        // GET /api/sessions?year=2025&week=30
        // GET /api/sessions?year=2025&month=7
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
            // 1) single‐date
            if (date.HasValue)
                return await GetByDate(date.Value, ct);

            // 2) date‐range
            if (start.HasValue && end.HasValue)
                return await GetByRange(start.Value, end.Value, ct);

            // 3) status
            if (status.HasValue)
                return await GetByStatus(status.Value, ct);

            // 4) ISO‐week (needs both year+week)
            if (year.HasValue && week.HasValue)
                return await GetByWeek(year.Value, week.Value, ct);

            // 5) month (needs both year+month)
            if (year.HasValue && month.HasValue)
                return await GetByMonth(year.Value, month.Value, ct);

            // 6) no recognized filter → 400 Bad Request
            return ApiResponseFactory.BadRequestResponse(
                "filters",
                "You must supply one of: date; start+end; status; year+week; or year+month."
            );
        }

        private Task<IActionResult> GetByDate(DateTime date, CancellationToken ct) =>
            Wrap(() => _sessionService.GetSessionsByDate(date, ct),
                 $"Sessions for {date:yyyy-MM-dd}");

        private Task<IActionResult> GetByRange(DateTime start, DateTime end, CancellationToken ct) =>
            Wrap(() => _sessionService.GetSessionsInRange(start, end, ct),
                 $"Sessions from {start:yyyy-MM-dd} to {end:yyyy-MM-dd}");

        private Task<IActionResult> GetByStatus(SessionStatus status, CancellationToken ct) =>
            Wrap(() => _sessionService.GetSessionsByStatus(status, ct),
                 $"Sessions with status {status}");

        private Task<IActionResult> GetByWeek(int year, int week, CancellationToken ct) =>
            Wrap(() => _sessionService.GetSessionsByWeek(year, week, ct),
                 $"Sessions for ISO week {week} of {year}");

        private Task<IActionResult> GetByMonth(int year, int month, CancellationToken ct) =>
            Wrap(() => _sessionService.GetSessionsByMonth(year, month, ct),
                 $"Sessions for {year}-{month:D2}");

        /// <summary>
        /// Centralizes try/catch + domain‐failure handling + success→200.
        /// </summary>
        private async Task<IActionResult> Wrap<T>(
            Func<Task<Result<T>>> op,
            string successMsg)
        {
            try
            {
                var result = await op();
                if (result.IsFailure)
                    return ApiResponseFactory.FromFailure(result, nameof(GetListAsync),"Session");

                return ApiResponseFactory.SuccessResponse(result.Value, successMsg);
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
