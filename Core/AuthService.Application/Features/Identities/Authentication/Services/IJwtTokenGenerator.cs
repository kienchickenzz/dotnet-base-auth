/**
 * IJwtTokenGenerator defines contract for JWT token generation.
 *
 * <p>Abstraction for token generation logic, implemented in Infrastructure layer.</p>
 */
namespace AuthService.Application.Features.Identities.Authentication.Services;

using System.Security.Claims;


/// <summary>
/// Service for generating and validating JWT tokens.
/// </summary>
public interface IJwtTokenGenerator
{
    /// <summary>
    /// Generates a JWT access token for the specified user.
    /// </summary>
    /// <param name="userId">User's unique identifier.</param>
    /// <param name="email">User's email address.</param>
    /// <param name="firstName">User's first name.</param>
    /// <param name="lastName">User's last name.</param>
    /// <param name="phoneNumber">User's phone number (optional).</param>
    /// <param name="imageUrl">User's profile image URL (optional).</param>
    /// <param name="ipAddress">Client IP address.</param>
    /// <returns>JWT access token string.</returns>
    string GenerateAccessToken(
        Guid userId,
        string email,
        string? firstName,
        string? lastName,
        string? phoneNumber,
        string? imageUrl,
        string ipAddress);

    /// <summary>
    /// Generates a cryptographically secure refresh token.
    /// </summary>
    /// <returns>Base64 encoded refresh token.</returns>
    string GenerateRefreshToken();

    /// <summary>
    /// Gets the refresh token expiration time based on configuration.
    /// </summary>
    /// <returns>DateTime when refresh token expires.</returns>
    DateTime GetRefreshTokenExpiryTime();

    /// <summary>
    /// Extracts claims principal from an expired JWT token.
    /// </summary>
    /// <param name="token">Expired JWT token.</param>
    /// <returns>ClaimsPrincipal if token is valid, null otherwise.</returns>
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}
