/**
 * AuthenticationErrors defines error constants for authentication operations.
 *
 * <p>Used with Result pattern for consistent error handling.</p>
 */
namespace AuthService.Application.Features.Identities.Authentication;

using AuthService.Domain.Common;


/// <summary>
/// Error definitions for authentication-related operations.
/// </summary>
public static class AuthenticationErrors
{
    /// <summary>Invalid email or password.</summary>
    public static readonly Error InvalidCredentials = new(
        "Authentication.InvalidCredentials",
        "Invalid email or password.");

    /// <summary>User account is not active.</summary>
    public static readonly Error UserNotActive = new(
        "Authentication.UserNotActive",
        "User account is not active. Please contact the administrator.");

    /// <summary>Email address has not been confirmed.</summary>
    public static readonly Error EmailNotConfirmed = new(
        "Authentication.EmailNotConfirmed",
        "Email address has not been confirmed.");

    /// <summary>Invalid or expired token.</summary>
    public static readonly Error InvalidToken = new(
        "Authentication.InvalidToken",
        "Invalid or expired token.");

    /// <summary>Invalid or expired refresh token.</summary>
    public static readonly Error InvalidRefreshToken = new(
        "Authentication.InvalidRefreshToken",
        "Invalid or expired refresh token.");
}
