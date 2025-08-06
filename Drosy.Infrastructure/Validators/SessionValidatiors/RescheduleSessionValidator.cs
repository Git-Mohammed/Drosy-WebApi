using FluentValidation;
using Drosy.Application.UseCases.Sessions.DTOs;
using Drosy.Domain.Shared.ErrorComponents.Sesstions;
using Drosy.Domain.Shared.System.Validation.Rules;
using Drosy.Domain.Shared.ErrorComponents.Validation;

namespace Drosy.Infrastructure.Validators.SessionValidatiors
{
    public class RescheduleSessionValidator : AbstractValidator<RescheduleSessionDTO>
    {
        public RescheduleSessionValidator()
        {
            // NewDate must not be in the past
            RuleFor(x => x.NewDate.Date)
                .GreaterThanOrEqualTo(DateTime.Today)
                .WithMessage(SessionErrors.ExpectedDateInThePast.Message);

            // NewStartTime must be before NewEndTime
            RuleFor(x => x)
                .Must(x => x.NewStartTime < x.NewEndTime)
                .WithMessage(SessionErrors.StartAfterEnd.Message);

            // Start/End times must match NewDate
            RuleFor(x => x)
                .Must(x =>
                    x.NewStartTime.Date == x.NewDate.Date &&
                    x.NewEndTime.Date == x.NewDate.Date)
                .WithMessage(SessionErrors.OutsideExpectedDate.Message);
        }
    }
}
