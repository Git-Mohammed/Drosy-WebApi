using Drosy.Domain.Entities;

namespace Drosy.Domain.Interfaces.Repository
{
    public interface IStudentRepository : IRepository<Student>
    {
        public Task<Student?> GetByIdAsync(int id);

        public Student? GetById(int id);
    }
}
