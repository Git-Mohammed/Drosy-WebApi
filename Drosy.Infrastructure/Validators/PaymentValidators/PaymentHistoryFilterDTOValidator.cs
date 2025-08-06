using Drosy.Application.UseCases.Payments.DTOs;
using FluentValidation;
using Drosy.Domain.Shared.ErrorComponents.Payments;

namespace Drosy.Application.Validators.Payments;

public class PaymentHistoryFilterDTOValidator : AbstractValidator<PaymentHistoryFilterDTO>
{
    public PaymentHistoryFilterDTOValidator()
    {
        // Ensure FromDate is provided
        RuleFor(x => x.FromDate)
            .NotEmpty()
            .WithMessage(PaymentErrors.DateRequired.Message);

        // Ensure ToDate is provided
        RuleFor(x => x.ToDate)
            .NotEmpty()
            .WithMessage(PaymentErrors.DateRequired.Message);

        // Ensure FromDate <= ToDate
        RuleFor(x => x)
            .Must(x => x.FromDate <= x.ToDate)
            .WithMessage(PaymentErrors.InvalidDateRange.Message);
    }
}
