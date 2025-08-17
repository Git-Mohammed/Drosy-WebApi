using Drosy.Application.UseCases.SystemSettings.DTOs;
using Drosy.Domain.Entities;
using Mapster;

namespace Drosy.Infrastructure.Mapping.SystemSettingConfigs
{
    public class SystemSettingMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // Map SystemSetting → SystemSettingDTO
            config.NewConfig<SystemSetting, SystemSettingDTO>();

            // Map UpdateSystemSettingDTO → SystemSetting (for Update operations)
            config.NewConfig<UpdateSystemSettingDTO, SystemSetting>()
                  .Ignore(dest => dest.LogoPath); // LogoPath is handled separately during file upload
        }
    }
}
