using FluentValidation;

namespace Drosy.Infrastructure.Validators
{
    public class SampleValidator : AbstractValidator<string>
    {
        public SampleValidator()
        {

            //var allowNames = new[] { "reda", "abdalla", "mery", "mohammed", "abdulaziz" };
            //RuleFor(x => x)
            //    .NotNull().WithMessage("Please provide a valid value")
            //    .NotEmpty().WithMessage("Name is required.")
            //    .Must(x => allowNames.Contains(x.ToLower()))
            //    .WithMessage("Name Is Not Allowed.");
        }
    }
}