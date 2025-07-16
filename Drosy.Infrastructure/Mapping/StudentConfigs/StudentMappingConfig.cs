using Mapster;
using Drosy.Domain.Entities;
using Drosy.Application.UseCases.Students.DTOs;
using Drosy.Application.UseCases.Grades.DTOs;

namespace Drosy.Infrastructure.Mapping.StudentConfigs
{
    public static class StudentMappingConfig
    {
        public static void RegisterMappings(TypeAdapterConfig config)
        {
            // Grade mapping
            config.NewConfig<Grade, GradeDTO>();

            // City mapping

            // Student mapping with nested DTOs
            config.NewConfig<Student, StudentDTO>()
                  .Map(dest => dest.Grade, src => src.Grade)
                  .Map(dest => dest.City, src => src.City);

            config.NewConfig<UpdateStudentDTO, Student>()
              .Ignore(dest => dest.Id)
              .Map(des => des.GradeId, src => src.GradeId)
              .Map(des => des.CityId, src => src.CityId);

            // Optional: Reverse mappings
            config.NewConfig<StudentDTO, Student>()
                  .Map(dest => dest.Grade, src => src.Grade)
                  .Map(dest => dest.City, src => src.City);


            config.NewConfig<Student, StudentCardInfoDTO>()
            .Map(dest => dest.FullName,
                 src => $"{src.FirstName} {src.ThirdName} {src.LastName}")
            .Map(dest => dest.Address, src => src.Address)
            .Map(dest => dest.PhoneNumber, src => src.PhoneNumber)
            .Map(dest => dest.Grade, src => src.Grade.Name)
            .Map(dest => dest.PlansCount, src => src.Plans.Count);


        }
    }
}
