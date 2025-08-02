using Drosy.Api.Commons.Responses;
using Drosy.Application.UseCases.Plans.DTOs;
using Drosy.Application.UseCases.Plans.Interfaces;
using Drosy.Application.UseCases.Sessions.DTOs;
using Drosy.Application.UseCases.Sessions.Interfaces;
using Drosy.Domain.Entities;
using Drosy.Domain.Enums;
using Drosy.Domain.Shared.ApplicationResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.NetworkInformation;

namespace Drosy.Api.Controllers;

/// <summary>
/// Controller for managing educational plans.
/// </summary>
[ApiController]
[Route("api/plans")]
[Authorize]
public class PlansController(IPlanService planService, ISessionService sessionService, ILogger<PlansController> logger) : ControllerBase
{
    private readonly IPlanService _planService = planService;
    private readonly ISessionService _sessionService = sessionService;
    private readonly ILogger<PlansController> _logger = logger;

    #region Read
    /// <summary>
    /// Retrieves a plan by its ID.
    /// </summary>
    /// <param name="id">The ID of the plan.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The details of the specified plan.</returns>
    [HttpGet("{id}", Name = "GetPlanById")]
    [ProducesResponseType(typeof(ApiResponse<PlanDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PlanDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var result = await _planService.GetPlanByIdAsync(id, cancellationToken);
        const string operation = nameof(GetByIdAsync);
        if (result.IsSuccess)
            return ApiResponseFactory.SuccessResponse(result.Value);

        _logger.LogError("Error retrieving plan [{id}]: {Error}", id, result.Error);
        return ApiResponseFactory.FromFailure(result, operation, nameof(PlanDto));
    }


    // GET /api/plans
    [HttpGet(Name = "GetPlans")]
    public async Task<IActionResult> GetListAsync(
        [FromQuery] DateTime? date,
        [FromQuery] DateTime? start,
        [FromQuery] DateTime? end,
        [FromQuery] PlanStatus? status,
        [FromQuery] PlanTypes? type,
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
                () => _planService.GetPlansByDate(date.Value, ct),
                $"Plans on {date:yyyy-MM-dd}",
                nameof(GetListAsync),
                "Plans");

        if (start.HasValue && end.HasValue)
            return await Wrappers.WrapListFilter(
                () => _planService.GetPlansInRange(start.Value, end.Value, ct),
                $"Plans from {start:yyyy-MM-dd} to {end:yyyy-MM-dd}",
                nameof(GetListAsync),
                "Plans");

        if (status.HasValue)
            return await Wrappers.WrapListFilter(
                () => _planService.GetPlansByStatus(status.Value, ct),
                $"Plans with status {status}",
                nameof(GetListAsync),
                "Plans");

        if (type.HasValue)
            return await Wrappers.WrapListFilter(
                () => _planService.GetPlansByType(type.Value, ct),
                $"Plans of type {type}",
                nameof(GetListAsync),
                "Plans");

        if (year.HasValue && week.HasValue)
            return await Wrappers.WrapListFilter(
                () => _planService.GetPlansByWeek(year.Value, week.Value, ct),
                $"Plans in ISO week {week}/{year}",
                nameof(GetListAsync),
                "Plans");

        if (year.HasValue && month.HasValue)
            return await Wrappers.WrapListFilter(
                () => _planService.GetPlansByMonth(year.Value, month.Value, ct),
                $"Plans in {year}-{month:D2}",
                nameof(GetListAsync),
                "Plans");

        // No filters: return all
        return await Wrappers.WrapListFilter(
            () => _planService.GetAllAsync(ct),
            "All plans",
            nameof(GetListAsync),
            "Plans");
    }

    //GET /api/plans/1/sessions? date = 2023 - 10 - 15
    //GET /api/plans/1/sessions? start = 2023 - 10 - 01 & end = 2023 - 10 - 07
    //GET /api/plans/1/sessions? planStatus = Active
    //GET /api/plans/1/sessions? year = 2023 & week = 42
    //GET /api/plans/1/sessions? year = 2023 & month = 10
    //GET /api/plans/1/sessions
    [HttpGet("{planId:int}/sessions", Name = "GetPlanSessions")]
    public async Task<IActionResult> GetPlanSessionsAsync(
        [FromRoute] int planId,
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
                () => _sessionService.GetSessionsByDate(planId, date.Value, ct),
                $"Sessions on {date:yyyy-MM-dd}",
                nameof(GetPlanSessionsAsync),
                "Sessions");

        if (start.HasValue && end.HasValue)
            return await Wrappers.WrapListFilter(
                () => _sessionService.GetSessionsInRange(planId, start.Value, end.Value, ct),
                $"Sessions from {start:yyyy-MM-dd} to {end:yyyy-MM-dd}",
                nameof(GetPlanSessionsAsync),
                "Sessions");

        if (status.HasValue)
            return await Wrappers.WrapListFilter(
                () => _sessionService.GetSessionsByStatus(planId, status.Value, ct),
                $"Sessions with status {status}",
                nameof(GetPlanSessionsAsync),
                "Sessions");

        if (year.HasValue && week.HasValue)
            return await Wrappers.WrapListFilter(
                () => _sessionService.GetSessionsByWeek(planId, year.Value, week.Value, ct),
                $"Sessions in ISO week {week}/{year}",
                nameof(GetPlanSessionsAsync),
                "Sessions");

        if (year.HasValue && month.HasValue)
            return await Wrappers.WrapListFilter(
                () => _sessionService.GetSessionsByMonth(planId, year.Value, month.Value, ct),
                $"Sessions in {year}-{month:D2}",
                nameof(GetPlanSessionsAsync),
                "Sessions");

        // No filters: return all
        return await Wrappers.WrapListFilter(
            () => _sessionService.GetSessionsByPlan(planId, ct),
            "All sessions",
            nameof(GetPlanSessionsAsync),
            "Sessions");
    }


    //GET /api/plans/1/sessionsظcalendar? date = 2023 - 10 - 15
    //GET /api/plans/1/sessions/calendar? start = 2023 - 10 - 01 & end = 2023 - 10 - 07
    //GET /api/plans/1/sessions/calendar? planStatus = Active
    //GET /api/plans/1/sessions/calendar? year = 2023 & week = 42
    //GET /api/plans/1/sessions/calendar? year = 2023 & month = 10
    //GET /api/plans/1/sessions/calendar
    [HttpGet("{planId:int}/sessions/calendar", Name = "GetPlanCalenderSessions")]
    public async Task<IActionResult> GetPlanCalendarSessionsAsync(
         [FromRoute] int planId,
         [FromQuery] DateTime? date,
         [FromQuery] DateTime? start,
         [FromQuery] DateTime? end,
         [FromQuery] PlanStatus? status,
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
                () => _planService.GetPlanSessionsCalenderByDateAsync(planId, date.Value, ct),
                $"Calendar sessions for plan {planId} on {date.Value:yyyy-MM-dd}",
                nameof(GetPlanCalendarSessionsAsync),
                "CalendarSessions");

        if (start.HasValue && end.HasValue)
            return await Wrappers.WrapListFilter(
                () => _planService.GetPlanSessionsCalenderByRangeAsync(planId, start.Value, end.Value, ct),
                $"Calendar sessions for plan {planId} from {start.Value:yyyy-MM-dd} to {end.Value:yyyy-MM-dd}",
                nameof(GetPlanCalendarSessionsAsync),
                "CalendarSessions");

        if (status.HasValue)
            return await Wrappers.WrapListFilter(
                () => _planService.GetPlanSessionsCalenderByStatusAsync(planId, status.Value, ct),
                $"Calendar sessions for plan {planId} with status {status.Value}",
                nameof(GetPlanCalendarSessionsAsync),
                "CalendarSessions");

        if (year.HasValue && week.HasValue)
            return await Wrappers.WrapListFilter(
                () => _planService.GetPlanSessionsCalenderByWeekAsync(planId, year.Value, week.Value, ct),
                $"Calendar sessions for plan {planId} in week {week.Value} of {year.Value}",
                nameof(GetPlanCalendarSessionsAsync),
                "CalendarSessions");

        if (year.HasValue && month.HasValue)
            return await Wrappers.WrapListFilter(
                () => _planService.GetPlanSessionsCalenderByMonthAsync(planId, year.Value, month.Value, ct),
                $"Calendar sessions for plan {planId} in {year.Value}-{month.Value:D2}",
                nameof(GetPlanCalendarSessionsAsync),
                "CalendarSessions");

        // No filters: return all
        return await Wrappers.WrapListFilter(
            () => _planService.GetPlanSessionsCalenderAsync(planId, ct),
            $"All calendar sessions for plan {planId}",
            nameof(GetPlanCalendarSessionsAsync),
            "CalendarSessions");
    }

    #endregion

    #region Write

    /// <summary>
    /// Retrieves all plans without their full details.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of all existing plans.</returns>
    [HttpGet(Name = "GetAllPlansWithoutDetails")]
    [ProducesResponseType(typeof(ApiResponse<List<PlanDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PlanDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAllPlansWithoutDetails(CancellationToken cancellationToken)
    {
        var result = await _planService.GetAllPlansAsync(cancellationToken);
        const string operation = nameof(GetAllPlansWithoutDetails);
        if (result.IsSuccess)
            return ApiResponseFactory.SuccessResponse(result.Value);

        _logger.LogError("Error retrieving plans: {Error}", result.Error);
        return ApiResponseFactory.FromFailure(result, operation, nameof(PlanDto));
    }

    /// <summary>
    /// Creates a new plan.
    /// </summary>
    /// <param name="newPlan">The plan to be created.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created plan and its ID.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<PlanDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<PlanDto>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAsync(CreatePlanDto newPlan, CancellationToken cancellationToken)
    {
        var result = await _planService.CreatePlanAsync(newPlan, cancellationToken);
        const string operation = nameof(CreateAsync);

        if (result.IsSuccess)
            return ApiResponseFactory.CreatedResponse("GetPlanById", new { id = result.Value.Id }, result.Value);

        _logger.LogError("Error creating new plan [{newPlan}]: {Error}", newPlan, result.Error);
        return ApiResponseFactory.FromFailure(result, operation, nameof(PlanDto));
    }

    /// <summary>
    /// Deletes a plan by ID.
    /// </summary>
    /// <param name="id">The ID of the plan to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Status of the delete operation.</returns>
    [HttpDelete("{id}", Name = "DeletePlanById")]
    [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteByIdAsync(int id, CancellationToken cancellationToken)
    {
        var result = await _planService.DeleteAsync(id, cancellationToken);
        const string operation = nameof(DeleteByIdAsync);

        if (result.IsSuccess)
            return ApiResponseFactory.SuccessResponse();

        _logger.LogError("Error deleting plan [{id}]: {Error}", id, result.Error);
        return ApiResponseFactory.FromFailure(result, operation, nameof(PlanDto));
    }

    /// <summary>
    /// Updates the status of an existing plan.
    /// </summary>
    /// <param name="id">The ID of the plan to update.</param>
    /// <param name="planStatus">The new status to assign.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Status of the update operation.</returns>
    [HttpPatch("{id}", Name = "UpdatePlanStatus")]
    [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdatePlanStatusAsync(int id, UpdatePlanStatusDto planStatus, CancellationToken cancellationToken)
    {
        var result = await _planService.UpdateStatusAsync(id, planStatus, cancellationToken);
        const string operation = nameof(UpdatePlanStatusAsync);

        if (result.IsSuccess)
            return ApiResponseFactory.SuccessResponse();

        _logger.LogError("Error updating plan [{id}]: {Error}", id, result.Error);
        return ApiResponseFactory.FromFailure(result, operation, nameof(PlanDto));
    }
    #endregion

}
