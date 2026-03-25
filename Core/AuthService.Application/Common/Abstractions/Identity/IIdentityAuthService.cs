/**
 * IIdentityAuthService defines contract for authentication operations.
 *
 * <p>Handles credential validation and refresh token management.</p>
 */
namespace AuthService.Application.Common.Abstractions.Identity;

using AuthService.Application.Common.Abstractions.Identity.Models;
using AuthService.Domain.Common;


/// <summary>
/// Service for authentication operations.
/// </summary>
public interface IIdentityAuthService
{
    /// <summary>
    /// Validates user credentials.
    /// </summary>
    /// <param name="email">User's email address.</param>
    /// <param name="password">User's password.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result containing user info or error.</returns>
    Task<Result<UserDto>> ValidateCredentialsAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets user by email for refresh token validation.
    /// </summary>
    /// <param name="email">User's email address.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result containing user info or error.</returns>
    Task<Result<UserDto>> GetUserByEmailAsync(
        string email,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates a refresh token for a user.
    /// </summary>
    /// <param name="userId">User's Id.</param>
    /// <param name="refreshToken">Refresh token to validate.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result indicating success or error.</returns>
    Task<Result> ValidateRefreshTokenAsync(
        Guid userId,
        string refreshToken,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates user's refresh token.
    /// </summary>
    /// <param name="userId">User's Id.</param>
    /// <param name="refreshToken">New refresh token.</param>
    /// <param name="expiryTime">Refresh token expiry time.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task UpdateRefreshTokenAsync(
        Guid userId,
        string refreshToken,
        DateTime expiryTime,
        CancellationToken cancellationToken = default);
}
