using Drosy.Application.Interfaces.Common;
using Drosy.Application.UsesCases.Authentication.DTOs;
using Drosy.Domain.Entities;
using Drosy.Domain.Interfaces.Common.Uow;
using Drosy.Domain.Interfaces.Repository;
using Drosy.Domain.Shared.ApplicationResults;
using Drosy.Domain.Shared.ErrorComponents;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Drosy.Domain.Shared.ErrorComponents.Common;
using Drosy.Domain.Shared.ErrorComponents.EFCore;
using Microsoft.Extensions.Options;

namespace Drosy.Infrastructure.JWT
{
    public class JwtService : IJwtService
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly AuthOptions _jwtOptions;
        private readonly ILogger<JwtService> _logger;
        public JwtService(IOptions<AuthOptions> jWToptions, IUnitOfWork unitOfWork, IRefreshTokenRepository refreshTokenRepository, ILogger<JwtService> logger)
        {
            _jwtOptions = jWToptions.Value;
            _unitOfWork = unitOfWork;
            _refreshTokenRepository = refreshTokenRepository;
            _logger = logger;
        }
        private List<Claim> GenerateUserClaims(string userName, int userId, List<string> userRoles)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            };

            if (userRoles is not null)
            {
                foreach (var role in userRoles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }

            return claims;
        }
        public SecurityToken GenerateToken(string userName, int userId, List<string> userRoles)
        {
            var claims = GenerateUserClaims(userName, userId, userRoles);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _jwtOptions.Issuer,
                Audience = _jwtOptions.Audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SigningKey)),
                    SecurityAlgorithms.HmacSha256),
                Expires = DateTime.Now.AddMinutes(_jwtOptions.Lifetime),
                Subject = new ClaimsIdentity(claims)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return token;
        }
        public string GenerateTokenString(string userName, int userId, List<string> userRoles)
        {
            var token = GenerateToken(userName, userId, userRoles);
            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }
        public async Task<Result<AuthModel>> CreateTokenAsync(AppUser user, CancellationToken cancellationToken)
        {
            if (user is null) return Result.Failure<AuthModel>(CommonErrors.NullValue);

            var tokenString = GenerateTokenString(user.UserName, user.Id, user.Roles);

            var token = new AuthModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                AccessToken = tokenString,
                Roles = user.Roles
            };


            var existingRefreshToken = await _refreshTokenRepository.GetByUserIdAsync(user.Id, cancellationToken);

            if (existingRefreshToken != null && existingRefreshToken.IsActive)
            {
                token.RefreshToken = existingRefreshToken.Token;
                token.RefreshTokenExpiration = existingRefreshToken.ExpiresOn;
            }
            else
            {
                var refreshToken = GenerateRefreshToken(user.Id);
                token.RefreshToken = refreshToken.Token;
                token.RefreshTokenExpiration = refreshToken.ExpiresOn;
                await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
                var saveingResult = await _unitOfWork.SaveChangesAsync(cancellationToken);

                if (!saveingResult)
                    return Result.Failure<AuthModel>(EfCoreErrors.CanNotSaveChanges);
            }

            return Result.Success(token);
        }
        public RefreshToken GenerateRefreshToken(int userId)
        {
            var randomNumber = new byte[32];

            using var generator = new RNGCryptoServiceProvider();

            generator.GetBytes(randomNumber);

            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                ExpiresOn = DateTime.UtcNow.AddDays(1),
                CreatedOn = DateTime.UtcNow,
                UserId = userId
            };
        }
        public async Task<Result<AuthModel>> RefreshTokenAsync(string tokenString, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                var refreshToken = await _refreshTokenRepository.GetByTokenAsync(tokenString, cancellationToken);

                if (refreshToken is null)
                    return Result.Failure<AuthModel>(AppError.Failure);



                if (refreshToken is null || refreshToken.User == null)
                {
                    return Result.Failure<AuthModel>(AppError.Failure);
                }


                refreshToken.RevokedOn = DateTime.UtcNow;
                await _refreshTokenRepository.UpdateAsync(refreshToken, cancellationToken);

                var newRefreshToken = GenerateRefreshToken(refreshToken.UserId);
                await _refreshTokenRepository.AddAsync(newRefreshToken, cancellationToken);

                var savingResult = await _unitOfWork.SaveChangesAsync(cancellationToken);

                if (!savingResult)
                    return Result.Failure<AuthModel>(EfCoreErrors.CanNotSaveChanges);

                var token = new AuthModel
                {
                    UserId = refreshToken.User.Id,
                    AccessToken = GenerateTokenString(refreshToken.User.UserName, refreshToken.User.Id, null),
                    UserName = refreshToken.User.UserName,
                    RefreshToken = newRefreshToken.Token,
                    RefreshTokenExpiration = newRefreshToken.ExpiresOn
                };

                return Result.Success(token);
            }

            catch (OperationCanceledException)
            {
                _logger.LogWarning(CommonErrors.OperationCancelled.Message, "Operation Canceld While refresing the token");
                return Result.Failure<AuthModel>(CommonErrors.OperationCancelled);
            }


        }

        public async Task<Result> RevokeRefreshTokensAsync(string refreshToken, CancellationToken cancellationToken)
        {
            var token = await _refreshTokenRepository.GetByTokenAsync(refreshToken, cancellationToken);
            if (token is null)
                return Result.Success();
            token.RevokedOn = DateTime.UtcNow;
            token.ExpiresOn = DateTime.Now;

            var result = await _unitOfWork.SaveChangesAsync(cancellationToken);
            if (result)
                return Result.Failure(EfCoreErrors.CanNotSaveChanges);
            return Result.Success();
        }
    }
}
