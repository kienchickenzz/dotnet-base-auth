/**
 * RefreshTokenCommand represents a token refresh request.
 *
 * <p>Used to obtain new access token using a valid refresh token.</p>
 */
namespace AuthService.Application.Features.Identities.Authentication.Commands.RefreshToken;

using AuthService.Application.Common.Messaging;
using AuthService.Application.Features.Identities.Authentication.Models.Responses;


/// <summary>
/// Command to refresh JWT tokens using a valid refresh token.
/// </summary>
/// <param name="Token">Expired JWT access token.</param>
/// <param name="RefreshToken">Valid refresh token.</param>
/// <param name="IpAddress">Client's IP address for new token claims.</param>
public sealed record RefreshTokenCommand(
    string Token,
    string RefreshToken,
    string IpAddress) : ICommand<TokenResponse>;
