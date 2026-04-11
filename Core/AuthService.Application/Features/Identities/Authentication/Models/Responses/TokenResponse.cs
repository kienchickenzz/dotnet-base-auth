/**
 * TokenResponse contains JWT token and refresh token information.
 *
 * <p>Returned after successful authentication or token refresh.</p>
 */
namespace AuthService.Application.Features.Identities.Authentication.Models.Responses;


/// <summary>
/// Response containing JWT access token and refresh token.
/// </summary>
/// <param name="Token">JWT access token for API authorization.</param>
/// <param name="RefreshToken">Refresh token for obtaining new access tokens.</param>
/// <param name="RefreshTokenExpiryTime">Expiration time of the refresh token.</param>
/// <param name="Roles">User's roles for client-side authorization checks.</param>
public sealed record TokenResponse(
    string Token,
    string RefreshToken,
    DateTime RefreshTokenExpiryTime,
    IReadOnlyList<string> Roles);
