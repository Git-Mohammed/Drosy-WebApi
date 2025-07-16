using Drosy.Application.UseCases.Plans.DTOs;
using Drosy.Domain.Entities;
using Drosy.Domain.Enums;
using Mapster;

namespace Drosy.Infrastructure.Mapping.PlanConfigs;

public class PlanMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CreatePlanDTo, Plan>()
            .Map(dest => dest.Status, src => GetPlanStatus(src.Status))
            .Map(dest => dest.Type, src => GetPlanTypes(src.Type))
            .Map(dest => dest.DaysOfWeek, src => GetDays(src.DaysOfWeek))
            .Map(dest => dest.EndDate, src => src.StartDate.AddDays(src.Period));
        config.NewConfig<Plan, PlanDto>()
            .Map(dest => dest.Status, src => src.Status.ToString())
            .Map(dest => dest.Type, src => src.Type.ToString())
            .Map(dest => dest.DaysOfWeek, src => GetDaysOfWeek(src.DaysOfWeek))
            .Map(dest => dest.SessionPeriod, src => (src.EndSession - src.StartSession).TotalHours)
            .Map(dest => dest.Period, src => (src.EndDate - src.StartDate).Days);
    }

    private PlanStatus GetPlanStatus(string status)
    {
        return status.ToUpper() switch
        {
            "ACTIVE" => PlanStatus.Active,
            "INACTIVE" => PlanStatus.Inactive,
            _ => PlanStatus.Non
        };
    }
    private PlanTypes GetPlanTypes(string type)
    {
        return type.ToLower() switch
        {
            "INDIVIDUAL" => PlanTypes.Individual,
            "GROUP" => PlanTypes.Group,
            _ => PlanTypes.Individual
        };
    }
    private Days GetDays(List<string> weekOfDays)
    {
        var result = Days.None;
        foreach (var day in weekOfDays)
            if(Enum.TryParse<Days>(day.Trim(), true, out var paresDay))
                result |= paresDay;
            else
                throw new ArgumentException("Invalid days provided");
        return result;
    }
    private List<string> GetDaysOfWeek(Days days)
    {
        var result = new List<string>();
        foreach (var day in Enum.GetValues<Days>())
        {
            if(days == Days.None || day == Days.None) continue;
            if(days.HasFlag(day))
                result.Add(day.ToString());
        }
        return result;
    }
}