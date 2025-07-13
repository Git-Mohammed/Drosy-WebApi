using Drosy.Application.UseCases.Students.DTOs;
using Drosy.Domain.Entities;
using Drosy.Domain.Shared.ResultPattern;

namespace Drosy.Application.UseCases.Students.Interfaces
{
    public interface IStudentService
    {
        Task<Result<StudentDTO>> AddAsync(AddStudentDTO dto, CancellationToken cancellationToken);
        Task<Result<StudentDTO>> GetByIdAsync(int id, CancellationToken cancellationToken);
    }
}