using Drosy.Domain.Entities;
using Drosy.Domain.Interfaces.Common.Repository;

namespace Drosy.Domain.Interfaces.Repository
{
    public interface IStudentRepository : IRepository<Student>
    {
        public Task<Student?> GetByIdAsync(int id, CancellationToken cancellationToken);
        public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken);
        public Task<List<Student>> GetAllStudentsInfoCardsAsync(CancellationToken cancellationToken);
    }
}
