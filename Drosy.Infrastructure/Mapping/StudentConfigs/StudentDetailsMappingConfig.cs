using Drosy.Application.UseCases.Students.DTOs;
using Drosy.Domain.Entities;
using Drosy.Domain.Enums;
using Mapster;

namespace Drosy.Infrastructure.Mapping.StudentConfigs
{
    public static class StudentDetailsMappingConfig
    {
        public static void RegisterMappings(TypeAdapterConfig config)
        {
            config.NewConfig<Student, StudentDetailsDto>()
                .Map(dest => dest.FullName,
                     src => $"{src.FirstName} {src.SecondName} {(string.IsNullOrWhiteSpace(src.ThirdName) ? "" : src.ThirdName + " ")}{src.LastName}")
                .Map(dest => dest.PersonalInfo.City, src => src.City.Name)
                .Map(dest => dest.PersonalInfo.Phone, src => src.PhoneNumber)
                .Map(dest => dest.PersonalInfo.EmergencyContact, src => src.EmergencyNumber)
                .Map(dest => dest.PersonalInfo.District, src => src.Address)
                .Map(dest => dest.AcademicInfo.Grade, src => src.Grade.Name)
                .Map(dest => dest.AcademicInfo.ActivePlans, src => src.Plans.Count(p => p.Plan.Type.Equals(PlanStatus.Active)))
                .Map(dest => dest.PaymentStats.PaidAmount, src => src.Payments.Sum(p => p.Amount))
                //.Map(dest => dest.PaymentStats.TotalAmount, src => src.Payments.Sum(p => p.TotalDue)) 
                //.Map(dest => dest.PaymentStats.DueAmount, src => src.Payments.Sum(p => p.Amount - p.src)) 
                .Map(dest => dest.LessonStats.CompletedLessons, src => src.Attendences.Count(a => a.Status.Equals(AttendenceStatus.Present)));
                //.Map(dest => dest.LessonStats.UpcomingLessons, src => src.Attendences.Count(a => a.IsUpcoming))
                //.Map(dest => dest.LessonStats.TotalLessons, src => src.Attendences.Count);
        }
    }

}
