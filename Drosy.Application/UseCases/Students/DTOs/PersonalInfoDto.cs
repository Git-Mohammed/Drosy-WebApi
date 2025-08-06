namespace Drosy.Application.UseCases.Students.DTOs
{
    public class PersonalInfoDto
    {
        public string Phone { get; set; } = null!;
        public string EmergencyContact { get; set; } = null!;
        public string City { get; set; } = null!;
        public string? District { get; set; }
    }

}