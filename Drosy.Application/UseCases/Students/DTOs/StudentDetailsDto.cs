using Drosy.Application.UseCases.Grades.DTOs;
using Drosy.Application.UseCases.Payments.DTOs;
using Drosy.Application.UseCases.Sessions.DTOs;

namespace Drosy.Application.UseCases.Students.DTOs
{
    public class StudentDetailsDto
    {
        public string FullName { get; set; } = null!;
        public PersonalInfoDto PersonalInfo { get; set; } = null!;
        public AcademicInfoDto AcademicInfo { get; set; } = null!;
        public PaymentStatsDto? PaymentStats { get; set; }
        public LessonStatsDto? LessonStats { get; set; }
    }

}