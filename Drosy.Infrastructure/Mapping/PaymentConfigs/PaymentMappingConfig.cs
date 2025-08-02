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

        // 🔍 Map Payment → PaymentDetailDTO for history
        config.NewConfig<Payment, PaymentDetailDTO>()
            .Map(dest => dest.Amount, src => src.Amount)
            .Map(dest => dest.Date, src => src.CreatedAt)
            .Map(dest => dest.Notes, src => src.Plan != null ? src.Notes : string.Empty)
            .Map(dest => dest.Method, src => src.Method); // Uses PaymentMethod enum
    }
}