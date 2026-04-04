/**
 * CustomerLoginCommand represents a web-based customer login request.
 *
 * <p>Processed by CustomerLoginCommandHandler for cookie-based authentication.</p>
 */
namespace AuthService.Application.Features.Identities.Authentication.Commands.CustomerLogin;

using AuthService.Application.Common.Messaging;

/// <summary>
/// Command to authenticate customer via web (cookie-based).
/// </summary>
/// <param name="Email">User's email address.</param>
/// <param name="Password">User's password.</param>
/// <param name="RememberMe">Whether to persist session.</param>
public sealed record CustomerLoginCommand(
    string Email,
    string Password,
    bool RememberMe) : ICommand<CustomerLoginResult>;


/// <summary>
/// Result of customer login attempt.
/// </summary>
/// <param name="Succeeded">Whether login succeeded.</param>
/// <param name="RequiresTwoFactor">Whether 2FA is required.</param>
/// <param name="IsLockedOut">Whether user is locked out.</param>
public sealed record CustomerLoginResult(
    bool Succeeded,
    bool RequiresTwoFactor,
    bool IsLockedOut);
