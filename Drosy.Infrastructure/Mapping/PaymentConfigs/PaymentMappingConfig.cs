using Drosy.Application.UseCases.Payments.DTOs;
using Drosy.Domain.Entities;
using Mapster;

namespace Drosy.Infrastructure.Mapping.PaymentConfigs;

public class PaymentMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Payment,PaymentDto>();
        config.NewConfig<CreatePaymentDto,Payment>();
    }
}