using Drosy.Application.UseCases.Plans.DTOs;
using Drosy.Application.UseCases.Sessions.DTOs;
using Drosy.Domain.Entities;
using Mapster;

namespace Drosy.Infrastructure.Mapping.SessionConfigs
{
    public class SessionMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // CreateSessionDTO → Session
            config.NewConfig<CreateSessionDTO, Session>()
                .Map(dest => dest.Title, src => src.Title.Trim())
                .Map(dest => dest.PlanId, src => src.PlanId)
                .Map(dest => dest.ExcepectedDate, src => src.ExcepectedDate)
                .Map(dest => dest.StartTime, src => src.StartTime)
                .Map(dest => dest.EndTime, src => src.EndTime);

            // RescheduleSessionDTO → Session
            config.NewConfig<RescheduleSessionDTO, Session>()
                .Map(dest => dest.ExcepectedDate, src => src.NewDate)
                .Map(dest => dest.StartTime, src => src.NewStartTime)
                .Map(dest => dest.EndTime, src => src.NewEndTime);

            // Session → SessionDTO
            TypeAdapterConfig<Session, SessionDTO>
                .NewConfig()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.Title, src => src.Title)
                .Map(dest => dest.ExcepectedDate, src => src.ExcepectedDate)
                .Map(dest => dest.StartTime, src => src.StartTime)
                .Map(dest => dest.EndTime, src => src.EndTime)
                .Map(dest => dest.Notes, src => src.Notes)
                .Map(dest => dest.Plan, src => src.Plan.Adapt<PlanDto>());
        }
    }
}
