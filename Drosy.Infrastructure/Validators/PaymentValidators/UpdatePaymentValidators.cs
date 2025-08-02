using Drosy.Application.UseCases.Payments.DTOs;
using FluentValidation;

namespace Drosy.Infrastructure.Validators.PaymentValidators;

public class UpdatePaymentValidators : AbstractValidator<UpdatePaymentDto>
{
    public UpdatePaymentValidators()
    {
        RuleFor(x => x.PlanId)
            .NotEmpty()
            .GreaterThan(0)
            .WithMessage("PlanId must be greater than zero");

        RuleFor(x => x.StudentId)
            .NotEmpty()
            .GreaterThan(0)
            .WithMessage("StudentId must be greater than zero");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than 0");

        RuleFor(x => x.Method)
            .IsInEnum()
            .WithMessage("Invalid payment method");

        RuleFor(x => x.Notes)
            .MaximumLength(500)
            .WithMessage("Notes cannot exceed 500 characters");
    }
}