using FluentValidation;
using Drosy.Application.UseCases.Sessions.DTOs;
using Drosy.Domain.Shared.ErrorComponents.Sesstions;
using Drosy.Domain.Shared.System.Validation.Rules;
using Drosy.Domain.Shared.ErrorComponents.Validation;

namespace Drosy.Infrastructure.Validators.SessionValidatiors
{
    public class AddSessionValidator : AbstractValidator<AddSessionDTO>
    {
        public AddSessionValidator()
        {
            // 📌 Title validation
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage(SessionErrors.TitleRequired.Message)
                .MaximumLength(TextValidationRules.MaxTitleLength).WithMessage(ValidationErrors.MaxLengthExceeded.Message);

            // 📌 PlanId must be positive
            RuleFor(x => x.PlanId)
                .GreaterThan(0).WithMessage(ValidationErrors.RequiredField.Message);

            // 📌 Expected Date must not be in the past
            RuleFor(x => x.ExcepectedDate.Date)
                .GreaterThanOrEqualTo(DateTime.Today).WithMessage(SessionErrors.ExpectedDateInThePast.Message);

            // 📌 StartTime must be before EndTime
            RuleFor(x => x)
                .Must(x => x.StartTime < x.EndTime)
                .WithMessage(SessionErrors.StartAfterEnd.Message);

            // 📌 Start/End must match ExpectedDate
            RuleFor(x => x)
                .Must(x =>
                    x.StartTime.Date == x.ExcepectedDate.Date &&
                    x.EndTime.Date == x.ExcepectedDate.Date)
                .WithMessage(SessionErrors.OutsideExpectedDate.Message);

            
        }
    }
}
