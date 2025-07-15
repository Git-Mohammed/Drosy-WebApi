using Mapster;
using Drosy.Domain.Entities;
using Drosy.Application.UseCases.PlanStudents.DTOs;

namespace Drosy.Infrastructure.Mapping.StudentConfigs
{
    /// <summary>
    /// Registers Mapster mappings for <see cref="PlanStudent"/>-related DTOs.
    /// </summary>
    public static class PlanStudentMappingConfig
    {
        /// <summary>
        /// Adds type adapters for mapping between domain entities and DTOs:
        /// <list type="bullet">
        ///   <item><description><see cref="PlanStudent"/> → <see cref="PlanStudentDto"/></description></item>
        ///   <item><description><see cref="AddStudentToPlanDto"/> → <see cref="PlanStudent"/></description></item>
        /// </list>
        /// </summary>
        /// <param name="config">The Mapster <see cref="TypeAdapterConfig"/> to which mappings are added.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="config"/> is <c>null</c>.</exception>
        public static void RegisterMappings(TypeAdapterConfig config)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

            // Map domain entity to DTO for read operations
            config.NewConfig<PlanStudent, PlanStudentDto>();

            // Map incoming DTO to domain entity for create/update operations
            config.NewConfig<AddStudentToPlanDto, PlanStudent>() ;

            config.NewConfig<AddStudentToPlanDto, PlanStudent>()
            // suppose you want to set `PlanId` from a parameter instead of from the DTO:
            .Map((src, dest, ctx) =>
            {
                // read the parameter "planId" that we'll pass at call‐time:
                dest.PlanId = (int)ctx.Parameters["planId"];
                dest.StudentId = src.StudentId;
                dest.Notes = src.Notes;
                dest.Fee = src.Fee;
    });

        }
    }
}
