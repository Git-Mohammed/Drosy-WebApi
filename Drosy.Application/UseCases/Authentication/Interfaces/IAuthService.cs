using Drosy.Application.UsesCases.Authentication.DTOs;
using Drosy.Application.UsesCases.Users.DTOs;
using Drosy.Domain.Shared.ApplicationResults;
using System.Security.Claims;

namespace Drosy.Application.UseCases.Authentication.Interfaces
{
    public interface IAuthService
    {
        /// <summary>
        /// Authenticates a user using the provided login credentials.
        /// </summary>
        /// <param name="user">The login data containing username and password.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>
        /// A result containing authentication details upon success or a failure result otherwise.
        /// </returns>
        Task<Result<AuthModel>> LoginAsync(UserLoginDTO user, CancellationToken cancellationToken);

        /// <summary>
        /// Refreshes an authentication token using a valid refresh token string.
        /// </summary>
        /// <param name="tokenString">The refresh token provided by the client.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>
        /// A result containing updated authentication details or a failure result.
        /// </returns>
        Task<Result<AuthModel>> RefreshTokenAsync(string tokenString, CancellationToken cancellationToken);

        /// <summary>
        /// Refresh user access token via refresh token
        /// </summary>
        /// <param name="user">The claims principal representing the user.</param>
        /// <param name="requiredRole">The required role to verify against.</param>
        /// <returns>
        /// True if the user is authorized with the given role; otherwise, false.
        /// </returns>
        bool IsAuthorized(ClaimsPrincipal user, string requiredRole);

        /// <summary>
        /// Logs out the user by invalidating the provided refresh token.
        /// </summary>
        /// <param name="refreshToken">The refresh token to be revoked.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>
        /// A result indicating success or failure of the logout operation.
        /// </returns>
        Task<Result> LogoutAsync(string refreshToken, CancellationToken cancellationToken);

        /// <summary>
        /// Changes the user's password based on the provided input and user ID.
        /// </summary>
        /// <param name="userId">The ID of the user requesting a password change.</param>
        /// <param name="dto">Data containing old, new, and confirmed password fields.</param>
        /// <param name="ct">Token to monitor for cancellation requests.</param>
        /// <returns>
        /// A result indicating success or failure of the password change.
        /// </returns>
        Task<Result> ChangePasswordAsync(int userId, ChangePasswordDTO dto, CancellationToken ct);

        /// <summary>
        /// Initiates the password reset process by sending an email with a reset link.
        /// </summary>
        /// <param name="email">The registered email address of the user.</param>
        /// <param name="link">The URL to include in the reset email.</param>
        /// <param name="ct">Token to monitor for cancellation requests.</param>
        /// <returns>
        /// A result indicating success or failure of the email dispatch.
        /// </returns>
        Task<Result> ForgetPasswordAsync(string email, string link, CancellationToken ct);

        /// <summary>
        /// Completes the password reset using a valid token and new credentials.
        /// </summary>
        /// <param name="dto">Data containing the reset token, new password, and confirmation.</param>
        /// <param name="ct">Token to monitor for cancellation requests.</param>
        /// <returns>
        /// A result indicating success or failure of the password reset.
        /// </returns>
        Task<Result> ResetPasswordAsync(RestPasswordDTO dto, CancellationToken ct);

    }
}
