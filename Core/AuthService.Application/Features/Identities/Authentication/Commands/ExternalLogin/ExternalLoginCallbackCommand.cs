/**
 * ExternalLoginCallbackCommand handles OAuth callback.
 *
 * <p>Attempts to sign in or returns info for new user registration.</p>
 */
namespace AuthService.Application.Features.Identities.Authentication.Commands.ExternalLogin;

using AuthService.Application.Common.Messaging;


/// <summary>
/// Command to handle external login callback from OAuth provider.
/// </summary>
/// <param name="IpAddress">Client IP address for token generation.</param>
public sealed record ExternalLoginCallbackCommand(string IpAddress)
    : ICommand<ExternalLoginCallbackResponse>;
