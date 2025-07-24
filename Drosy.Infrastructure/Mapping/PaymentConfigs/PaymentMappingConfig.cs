using Drosy.Application.UseCases.Payments.DTOs;
using Drosy.Domain.Entities;
using Mapster;

namespace Drosy.Infrastructure.Mapping.PaymentConfigs;

public class PaymentMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Payment,PaymentDto>()
            .Map(dest => dest.PaymentDate, src => src.CreatedAt);
        config.NewConfig<CreatePaymentDto,Payment>();
    }
}