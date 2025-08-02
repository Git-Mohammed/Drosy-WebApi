using Drosy.Application.UsesCases.Users.DTOs;
using FluentValidation;

namespace Drosy.Infrastructure.Validators.UserValidatiors
{
    public class ForgetPasswordDTOValidatior : AbstractValidator<ForgetPasswordDTO>
    {
        public ForgetPasswordDTOValidatior()
        {
            RuleFor(x => x.Email)
               .NotEmpty().WithMessage("Email is required.")
               .EmailAddress().WithMessage("A valid email address is required.");

            RuleFor(x => x.Link).NotEmpty().WithMessage("link should be provided");
        }

    }
}
