/**
 * LogoutCommand revokes user's refresh token.
 *
 * <p>Invalidates the current session by clearing refresh token.</p>
 */
namespace AuthService.Application.Features.Identities.Authentication.Commands.Logout;

using AuthService.Application.Common.Messaging;


/// <summary>
/// Command to logout user and revoke refresh token.
/// </summary>
public sealed record LogoutCommand(Guid UserId) : ICommand;
