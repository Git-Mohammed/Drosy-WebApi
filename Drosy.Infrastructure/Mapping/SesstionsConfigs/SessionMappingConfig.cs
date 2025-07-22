using Drosy.Application.UseCases.Sessions.DTOs;
using Drosy.Domain.Entities;
using Mapster;

namespace Drosy.Infrastructure.Mapping.SessionConfigs
{
    public class SessionMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // DTO ➜ Entity
            config.NewConfig<AddSessionDTO, Session>()
                .Map(dest => dest.Title, src => src.Title.Trim())
                .Map(dest => dest.PlanId, src => src.PlanId)
                .Map(dest => dest.ExcepectedDate, src => src.ExcepectedDate)
                .Map(dest => dest.StartTime, src => src.StartTime)
                .Map(dest => dest.EndTime, src => src.EndTime);

            // Entity ➜ DTO
            config.NewConfig<Session, SessionDTO>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.Plan.Id, src => src.PlanId)
                .Map(dest => dest.Title, src => src.Title)
                .Map(dest => dest.ExcepectedDate, src => src.ExcepectedDate)
                .Map(dest => dest.StartTime, src => src.StartTime)
                .Map(dest => dest.EndTime, src => src.EndTime);
        }
    }
}
