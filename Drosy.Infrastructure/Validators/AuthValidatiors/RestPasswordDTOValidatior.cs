using Drosy.Application.UsesCases.Users.DTOs;
using FluentValidation;

namespace Drosy.Infrastructure.Validators.UserValidatiors
{
    public class RestPasswordDTOValidatior : AbstractValidator<RestPasswordDTO>
    {
        public RestPasswordDTOValidatior()
        {
            RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("New Password is required.")
            .MinimumLength(8).WithMessage("New Password must be at least 8 characters long.")
            .Matches(@"[A-Z]").WithMessage("New Password must contain at least one uppercase letter.")
            .Matches(@"[a-z]").WithMessage("New Password must contain at least one lowercase letter.")
            .Matches(@"\d").WithMessage("New Password must contain at least one number.")
            .Matches(@"[\W]").WithMessage("New Password must contain at least one special character.");

            RuleFor(x => x.ConfirmedPassword).Equal(x => x.NewPassword).WithMessage("Confirmed Password Should match New Password");
            
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email address is required.");
        }

    }
}
