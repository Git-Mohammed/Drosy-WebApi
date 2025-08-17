using Drosy.Application.UseCases.Payments.DTOs;
using Drosy.Domain.Shared.ErrorComponents.Payments;
using Drosy.Domain.Shared.ErrorComponents.Validation;
using FluentValidation;

namespace Drosy.Infrastructure.Validators.PaymentValidators;

public class CreatePaymentValidator : AbstractValidator<CreatePaymentDto>
{
    public CreatePaymentValidator()
    {
        RuleFor(p => p.PlanId)
            .NotNull().WithMessage(ValidationErrors.RequiredField.Message)
            .NotEmpty().WithMessage(ValidationErrors.RequiredField.Message);
        RuleFor(p => p.StudentId)
            .NotNull().WithMessage(ValidationErrors.RequiredField.Message)
            .NotEmpty().WithMessage(ValidationErrors.RequiredField.Message);
        RuleFor(p => p.Amount)
            .NotNull().WithMessage(ValidationErrors.RequiredField.Message)
            .NotEmpty().WithMessage(ValidationErrors.RequiredField.Message)
            .GreaterThan(0).WithMessage("Amount must be greater than zero.");
    }
}