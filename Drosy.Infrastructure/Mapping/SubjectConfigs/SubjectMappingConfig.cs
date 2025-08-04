using Drosy.Application.UseCases.Subjects.DTOs;
using Drosy.Domain.Entities;
using Mapster;

namespace Drosy.Infrastructure.Mapping.SubjectConfigs
{
    public class SubjectMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // Map Subject → SubjectDTO
            config.NewConfig<Subject, SubjectDTO>();

            // Map CreateSubjectDTO → Subject (for Create operations)
            config.NewConfig<CreateSubjectDTO, Subject>();

            // Map UpdateSubjectDTO → Subject (for Update operations)
            config.NewConfig<UpdateSubjectDTO, Subject>();
        }
    }
}
