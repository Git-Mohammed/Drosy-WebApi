using Drosy.Application.UseCases.Cities.DTOs;
using Drosy.Application.UseCases.Grades.DTOs;

namespace Drosy.Application.UseCases.Students.DTOs
{
    public class StudentDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string SecondName { get; set; } = null!;
        public string? ThirdName { get; set; } = string.Empty;
        public string LastName { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string EmergencyNumber { get; set; } = null!;
        public GradeDTO Grade { get; set; } = null!;
        public CityDTO City { get; set; } = null!;
    }
}
