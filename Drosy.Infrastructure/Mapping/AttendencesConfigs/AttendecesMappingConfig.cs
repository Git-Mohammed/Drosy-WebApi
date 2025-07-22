using Mapster;
using Drosy.Domain.Entities;
using Drosy.Application.UseCases.Attendences.DTOs;

namespace Drosy.Infrastructure.Mapping.AttendencesConfigs
{
    public static class AttendecesMappingConfig
    {
        public static void RegisterMappings(TypeAdapterConfig config)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

            config.NewConfig<Attendence, AttendenceDto>();

            config.NewConfig<AddAttendencenDto, Attendence>();

            config.NewConfig<UpdateAttendencenDto, Attendence>();
        }
    }
}
