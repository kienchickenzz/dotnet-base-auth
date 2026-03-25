/**
 * IAuthenticationService defines contract for authentication operations.
 *
 * <p>Abstracts user validation and token management, implemented in Infrastructure.</p>
 */
namespace AuthService.Application.Features.Identities.Authentication.Services;

using AuthService.Domain.Common;


/// <summary>
/// Service for user authentication operations.
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Validates user credentials.
    /// </summary>
    /// <param name="email">User's email address.</param>
    /// <param name="password">User's password.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result containing authenticated user info or error.</returns>
    Task<Result<AuthenticatedUserInfo>> ValidateCredentialsAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets user info by email for refresh token validation.
    /// </summary>
    /// <param name="email">User's email address.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result containing user info or error.</returns>
    Task<Result<AuthenticatedUserInfo>> GetUserByEmailAsync(
        string email,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates a refresh token for a user.
    /// </summary>
    /// <param name="userId">User's ID.</param>
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
    /// <param name="userId">User's ID.</param>
    /// <param name="refreshToken">New refresh token.</param>
    /// <param name="expiryTime">Refresh token expiry time.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task UpdateRefreshTokenAsync(
        Guid userId,
        string refreshToken,
        DateTime expiryTime,
        CancellationToken cancellationToken = default);
}


/// <summary>
/// Authenticated user information returned after credential validation.
/// </summary>
public sealed record AuthenticatedUserInfo(
    Guid Id,
    string Email,
    string? FirstName,
    string? LastName,
    string? PhoneNumber,
    string? ImageUrl);
