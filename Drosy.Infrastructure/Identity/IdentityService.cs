using System.Diagnostics.CodeAnalysis;
using Drosy.Application.Interfaces.Common;
using Drosy.Application.UseCases.Email.Interfaces;
using Drosy.Application.UsesCases.Authentication.DTOs;
using Drosy.Application.UsesCases.Users.DTOs;
using Drosy.Domain.Entities;
using Drosy.Domain.Interfaces.Common.Uow;
using Drosy.Domain.Interfaces.Repository;
using Drosy.Domain.Shared.ApplicationResults;
using Drosy.Domain.Shared.ErrorComponents.Common;
using Drosy.Domain.Shared.ErrorComponents.User;
using Drosy.Infrastructure.Helper.Email;
using Drosy.Infrastructure.Helper.PasswordResetToken;
using Drosy.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace Drosy.Infrastructure.Identity
{
    public class IdentityService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ILogger<IdentityService> logger,
        RoleManager<ApplicationRole> roleManager, IMapper mapper, IPasswordResetTokenRepository passwordResetTokenRepository, IUnitOfWork unitOfWork, IEmailService emailService)
        : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
        private readonly IMapper _mapper = mapper;
        private readonly IPasswordResetTokenRepository _passwordResetTokenRepository = passwordResetTokenRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IEmailService _emailService = emailService;
        private readonly ILogger<IdentityService> _logger = logger;


        public Task<bool> CreateUserAsync(string username, string password)
        {
            _userManager.CreateAsync(new ApplicationUser { UserName = username, Email = username }, password);
            return Task.FromResult(true);
        }

        public async Task<Result<AppUser>> PasswordSignInAsync(string username, string password, bool isPersistent, bool lockoutOnFailure)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(username);
                if (user is null)
                    return Result.Failure<AppUser>(UserErrors.InvalidCredentials);

                var result = await _signInManager.PasswordSignInAsync(user, password, isPersistent, lockoutOnFailure);
                if (result.IsLockedOut)
                    return Result.Failure<AppUser>(UserErrors.AttemptExceeded);

                if (!result.Succeeded)
                    return Result.Failure<AppUser>(UserErrors.InvalidCredentials);

                var roles = await _userManager.GetRolesAsync(user);

                var appUser = _mapper.Map<ApplicationUser, AppUser>(user);
                appUser.Roles = [..roles];

                return Result.Success(appUser);
            }
            catch (Exception ex) 
            {
                _logger.LogError("un expected error occure, {message}", ex.Message);
                return Result.Failure<AppUser>(CommonErrors.Failure);
            }

        }

        public async Task<Result> ChangePasswordAsync(int userId, string oldPassword, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return Result.Failure(CommonErrors.NullValue);

            var passwordCheck = await _userManager.CheckPasswordAsync(user, oldPassword);
            if (!passwordCheck)
                return Result.Failure(UserErrors.InvalidCredentials);

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);

            if (!changePasswordResult.Succeeded)
                return Result.Failure(CommonErrors.Failure);

            return Result.Success();
        }

        public async Task<Result> ForgetPasswordAsync(string email, string link, CancellationToken ct)
        {
            try
            {
                ct.ThrowIfCancellationRequested();

                var user = await _userManager.FindByEmailAsync(email);

                if (user == null) 
                    return Result.Failure(CommonErrors.NullValue);

                var stringToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passwordToken = PasswordResetTokenHelper.CreateToken(user.Id, stringToken);

                await _passwordResetTokenRepository.AddAsync(passwordToken, ct);
                var IsSaved = await _unitOfWork.SaveChangesAsync(ct);
                if (!IsSaved)
                    return Result.Failure(CommonErrors.Failure);

                var emailBody = EmailTemplates.GetEmailConfirmEmailBody($"{link}token={passwordToken.TokenString}", user.UserName, 30);
                var emailMessageResult = await _emailService.SendEmailAsync(new Application.UseCases.Email.DTOs.EmailMessageDTO { Body = emailBody, RecipientEmail = email, RecipientName = $"{user.UserName}", Subject = "إعادة تعيين كلمة المرور"}, ct);
                if (emailMessageResult.IsFailure)
                    return Result.Failure(emailMessageResult.Error);

                return Result.Success();
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning(CommonErrors.OperationCancelled.Message, "Operation Canceld While refresing the token");
                return Result.Failure<AuthModel>(CommonErrors.OperationCancelled);
            }

        }

        public async Task<Result> RestPasswordAsync(RestPasswordDTO dto, CancellationToken ct)
        {
            try
            {
                ct.ThrowIfCancellationRequested();
                var restToken  = await _passwordResetTokenRepository.GetTokenAsync(dto.Token, ct);
                var user = await _userManager.FindByIdAsync(restToken.UserId.ToString());
                if (restToken == null)
                    return Result.Failure(CommonErrors.NullValue);

                restToken.IsUsed = true;

                var restResult = await _userManager.ResetPasswordAsync(user, restToken.TokenString, dto.NewPassword);

                if (!restResult.Succeeded)
                    return Result.Failure(CommonErrors.Failure);

                return Result.Success();    
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning(CommonErrors.OperationCancelled.Message, "Operation Canceld While rest user password");
                return Result.Failure<AuthModel>(CommonErrors.OperationCancelled);
            }
        }


    }
}