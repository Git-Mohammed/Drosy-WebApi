using Mapster;
using Drosy.Domain.Entities;
using Drosy.Application.UseCases.Grades.DTOs;

namespace Drosy.Infrastructure.Mapping.StudentConfigs
{
    public static class GradeMappingConfig
    {
        public static void RegisterMappings(TypeAdapterConfig config)
        {
            // Grade mapping
            config.NewConfig<Grade, GradeDTO>();
        }
    }
}
