using Drosy.Application.UseCases.Schedule.DTOs;
using Drosy.Domain.Shared.ApplicationResults;

namespace Drosy.Application.UseCases.Schedule.interfaces
{
    public interface ICalenderService
    {
        // Per‐plan calender:
        Task<Result<DataResult<CalenderSessionDto>>> GetPlanSessionsCalenderAsync(
            int planId, CancellationToken ct);

        // Global with no filter:
        Task<Result<DataResult<CalenderSessionDto>>> GetSessionsCalenderAsync(CancellationToken ct);

        // Global calender filters:
        Task<Result<DataResult<CalenderSessionDto>>> GetSessionsCalenderByDateAsync(
            DateTime date, CancellationToken ct);

        Task<Result<DataResult<CalenderSessionDto>>> GetSessionsCalenderInRangeAsync(
            DateTime start, DateTime end, CancellationToken ct);

        Task<Result<DataResult<CalenderSessionDto>>> GetSessionsCalenderByWeekAsync(
            int year, int week, CancellationToken ct);

        Task<Result<DataResult<CalenderSessionDto>>> GetSessionsCalenderByMonthAsync(
            int year, int month, CancellationToken ct);
    }

}
