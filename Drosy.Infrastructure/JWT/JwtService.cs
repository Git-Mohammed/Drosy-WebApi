using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Drosy.Application.Interfaces.Common;
using Drosy.Application.UsesCases.Authentication.DTOs;
using Drosy.Domain.Entities;
using Drosy.Domain.Interfaces.Repository;
using Drosy.Domain.Interfaces.Uow;
using Drosy.Domain.Shared.ResultPattern;
using Drosy.Domain.Shared.ResultPattern.ErrorComponents;
using Microsoft.IdentityModel.Tokens;

namespace Drosy.Infrastructure.JWT
{
    public class JwtService : IJwtService
    {
        private readonly IAppUserRepository _userRepository;
        private readonly IIdentityService _identityService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly AuthOptions _jwtOptions;
        public JwtService(AuthOptions jWToptions, IUnitOfWork unitOfWork, IRefreshTokenRepository refreshTokenRepository, IIdentityService identityService, IAppUserRepository userRepository)
        {
            _jwtOptions = jWToptions;
            _unitOfWork = unitOfWork;
            _refreshTokenRepository = refreshTokenRepository;
            _identityService = identityService;
            _userRepository = userRepository;
        }
        private List<Claim> GenerateUserClaims(string userName, int userId)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            };

            return claims;
        }
        public SecurityToken GenerateToken(string userName, int userId)
        {
            var claims = GenerateUserClaims(userName, userId);
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
        public string GenerateTokenString(string userName, int userId)
        {
            var token = GenerateToken(userName, userId);
            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }
        public async Task<Result<AuthModel>> CreateTokenAsync(AppUser user, CancellationToken cancellationToken)
        {
            if (user is null) return Result.Failure<AuthModel>(Error.NullValue);


            var tokenString = GenerateTokenString(user.UserName, user.Id);
            
            var token = new AuthModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                AccessToken = tokenString
            };


            var existingRefreshToken = await _refreshTokenRepository.GetByUserIdAsync(user.Id);

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
                await _refreshTokenRepository.AddAsync(refreshToken);
                var saveingResult = await _unitOfWork.SaveChangesAsync(cancellationToken);

                if (saveingResult <= 0)
                    return Result.Failure<AuthModel>(Error.EFCore.CanNotSaveChanges);
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

            var refreshToken = await _refreshTokenRepository.GetByTokenAsync(tokenString);

            if (refreshToken is null)
                return Result.Failure<AuthModel>(Error.Failure);



            if (refreshToken is null || refreshToken.User == null)
            {
                return Result.Failure<AuthModel>(Error.Failure);
            }


            refreshToken.RevokedOn = DateTime.UtcNow;
            await _refreshTokenRepository.UpdateAsync(refreshToken);

            var newRefreshToken = GenerateRefreshToken(refreshToken.UserId);
            await _refreshTokenRepository.AddAsync(newRefreshToken);

            var savingResult = await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (savingResult <= 0)
                return Result.Failure<AuthModel>(Error.EFCore.CanNotSaveChanges);

            var token = new AuthModel  {
                UserId = refreshToken.User.Id,
                AccessToken = GenerateTokenString(refreshToken.User.UserName, refreshToken.User.Id),
                UserName = refreshToken.User.UserName,
                RefreshToken = newRefreshToken.Token,
                RefreshTokenExpiration = newRefreshToken.ExpiresOn
            };

            return Result.Success(token);
        }

        public async Task<Result> RevokeRefreshTokensAsync(string refreshToken, CancellationToken cancellationToken)
        {
            var token = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
            if (token is null)
                return Result.Success();
            token.RevokedOn = DateTime.UtcNow;
            token.ExpiresOn = DateTime.Now;
            
            var result = await _unitOfWork.SaveChangesAsync(cancellationToken);
            if (result <= 0)
                return Result.Failure(Error.EFCore.CanNotSaveChanges);
            return Result.Success();
        }
    }
}
