using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drosy.Application.UsesCases.Users.DTOs;
using FluentValidation;

namespace Drosy.Infrastructure.Validators.UserValidatiors
{
    public class ChangePasswordDTOValidatior : AbstractValidator<ChangePasswordDTO> 
    {
        public ChangePasswordDTOValidatior()
        {
            RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches(@"\d").WithMessage("Password must contain at least one number.")
            .Matches(@"[\W]").WithMessage("Password must contain at least one special character.");

            RuleFor(x => x.ConfirmedPassword).Equal(x => x.NewPassword).WithMessage("Confirmed Password Should match New Password");

            RuleFor(x => x.OldPassword).NotEmpty().WithMessage("Old Password should not be empty");
        }
    }
}
