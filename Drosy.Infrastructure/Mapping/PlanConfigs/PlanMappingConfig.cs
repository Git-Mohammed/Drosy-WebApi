using Drosy.Application.UseCases.Plans.DTOs;
using Drosy.Domain.Entities;
using Drosy.Domain.Enums;
using Mapster;

namespace Drosy.Infrastructure.Mapping.PlanConfigs;

public class PlanMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CreatePlanDto, Plan>()
            .Map(dest => dest.Status, src => GetPlanStatus(src.Status))
            .Map(dest => dest.Type, src => GetPlanTypes(src.Type))
            .Map(dest => dest.EndDate, src => src.StartDate.AddDays(src.Period))
            .Map(dest => dest.PlanDays, src => src.Days.Select(dayDto => new PlanDay
            {
                Day = ParseDay(dayDto.Day),
                StartSession = dayDto.StartSession,
                EndSession = dayDto.EndSession
            }).ToList());

        config.NewConfig<Plan, PlanDto>()
            .Map(dest => dest.Status, src => src.Status.ToString())
            .Map(dest => dest.Type, src => src.Type.ToString())
            .Map(dest => dest.Days, src => src.PlanDays.Select(pd => new PlanDayDto
            {
                Day = pd.Day.ToString(),
                StartSession = pd.StartSession,
                EndSession = pd.EndSession
            }).ToList())
            .Map(dest => dest.SessionPeriod, src =>
                src.PlanDays.Any()
                    ? src.PlanDays.Average(pd => (pd.EndSession - pd.StartSession).TotalHours)
                    : 0)
            .Map(dest => dest.Period, src => (src.EndDate - src.StartDate).Days);

        #region PlanDay

        config.NewConfig<PlanDay, PlanDayDto>()
            .Map(dest => dest.Day, src => src.Day.ToString());
        config.NewConfig<PlanDayDto, PlanDay>()
            .Map(dest=> dest.Day, src => ParseDay(src.Day));
        #endregion
    }

    private PlanStatus GetPlanStatus(string status) =>
        status.ToUpper() switch
        {
            "ACTIVE" => PlanStatus.Active,
            "INACTIVE" => PlanStatus.Inactive,
            _ => PlanStatus.Non
        };

    private PlanTypes GetPlanTypes(string type) =>
        type.ToLower() switch
        {
            "individual" => PlanTypes.Individual,
            "group" => PlanTypes.Group,
            _ => PlanTypes.Individual
        };

    private Days ParseDay(string day)
    {
        if (Enum.TryParse<Days>(day.Trim(), true, out var parsedDay))
            return parsedDay;
        throw new ArgumentException($"Invalid day: {day}");
    }
}
