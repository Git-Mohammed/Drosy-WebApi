namespace Drosy.Application.UseCases.Students.DTOs
{
    public class UpdateStudentDTO
    {
        public string FirstName { get; set; } = null!;
        public string SecondName { get; set; } = null!;
        public string? ThirdName { get; set; } = string.Empty;
        public string LastName { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string EmergencyNumber { get; set; } = null!;
        public int GradeId { get; set; }
        public int? UserId { get; set; }
        public int CityId { get; set; }
    }
}