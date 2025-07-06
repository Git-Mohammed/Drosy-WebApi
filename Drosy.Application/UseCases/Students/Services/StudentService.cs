using Drosy.Domain.Interfaces.Repository;
using Drosy.Application.UseCases.Students.Interfaces;

namespace Drosy.Application.UseCases.Students.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;

        public StudentService(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }


    }
}
