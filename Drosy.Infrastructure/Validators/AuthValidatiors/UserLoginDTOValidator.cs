using Drosy.Application.UsesCases.Users.DTOs;
using FluentValidation;

namespace Drosy.Infrastructure.Validators.AuthValidatiors
{
    public class UserLoginDTOValidator : AbstractValidator<UserLoginDTO>
    {
        public UserLoginDTOValidator()
        {
            RuleFor(x => x.UserName).NotEmpty().WithMessage("UserName is required.");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.");
        }
    }
}
