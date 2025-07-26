using Mapster;
using Drosy.Domain.Entities;
using Drosy.Application.UseCases.Attendences.DTOs;

namespace Drosy.Infrastructure.Mapping.AttendencesConfigs
{
    /// <summary>
    /// Configuration class for mapping Attendence-related DTOs and entities using Mapster.
    /// </summary>
    public static class AttendecesMappingConfig
    {
        /// <summary>
        /// Registers mapping configurations between Attendence entity and related DTOs.
        /// </summary>
        /// <param name="config">The Mapster TypeAdapterConfig instance to register mappings on.</param>
        public static void RegisterMappings(TypeAdapterConfig config)
        {
            // Map from Attendence entity to AttendenceDto
            config.NewConfig<Attendence, AttendenceDto>();

            // Map from AddAttendencenDto to Attendence entity
            config.NewConfig<AddAttendencenDto, Attendence>();

            // Map from UpdateAttendencenDto to Attendence entity
            config.NewConfig<UpdateAttendencenDto, Attendence>();
        }
    }
}
