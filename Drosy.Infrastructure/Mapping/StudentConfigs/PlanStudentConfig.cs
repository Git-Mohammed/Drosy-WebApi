using Mapster;
using Drosy.Domain.Entities;
using Drosy.Application.UseCases.PlanStudents.DTOs;

namespace Drosy.Infrastructure.Mapping.StudentConfigs
{
    public static class PlanStudentConfig
    {
        public static void RegisterMappings(TypeAdapterConfig config)
        {
            config.NewConfig<PlanStudent, PlanStudentDto>();
            config.NewConfig<AddStudentToPlanDto, PlanStudent>();

            //       config.NewConfigWithContext<AddStudentToPlanDto, PlanStudent>()
            //.MapWith((src, context) =>
            //{
            //    // context is of type TypeAdapterContext, so to get parameters:
            //    var planId = context.Parameters["planId"] as Guid? ?? Guid.Empty;

            //    return new PlanStudent
            //    {
            //        StudentId = src.StudentId,
            //        PlanId = planId
            //        // map other props as needed
            //    };
            //});
        }
    }
}
