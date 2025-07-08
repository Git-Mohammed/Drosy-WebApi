using Mapster;
using Drosy.Domain.Entities;
using Drosy.Application.UseCases.Cities.DTOs;

namespace Drosy.Infrastructure.Mapping.StudentConfigs
{
    public static class CityMappingConfig
    {
        public static void RegisterMappings(TypeAdapterConfig config)
        {
            // Grade mapping
            config.NewConfig<City, CityDTO>();
        }
    }
}
