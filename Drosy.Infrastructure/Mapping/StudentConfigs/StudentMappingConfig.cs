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

            // Optional: Reverse mappings
            config.NewConfig<StudentDTO, Student>()
                  .Map(dest => dest.Grade, src => src.Grade)
                  .Map(dest => dest.City, src => src.City);
        }
    }
}
