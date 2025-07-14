using Drosy.Application.UseCases.PlanStudents.DTOs;
using Drosy.Domain.Shared.ResultPattern;

namespace Drosy.Application.UseCases.PlanStudents.Interfaces
{
    public interface IPlanStudentsService
    {
        Task<Result<PlanStudentDto>> AddStudentToPlanAsync(int planId, AddStudentToPlanDto dto,CancellationToken ct);
        Task<Result> AddRangeOfStudentToPlanAsync(int planId, IEnumerable<AddStudentToPlanDto> dto, CancellationToken c);
    }
}
