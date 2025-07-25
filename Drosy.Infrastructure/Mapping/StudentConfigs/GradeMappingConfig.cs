using Drosy.Application.UseCases.Grades.DTOs;
using Drosy.Domain.Entities;
using Mapster;

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
