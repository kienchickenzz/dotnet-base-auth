/**
 * ExternalLoginConfirmationCommand creates user from external provider.
 *
 * <p>Called when new user confirms their info after OAuth callback.</p>
 */
namespace AuthService.Application.Features.Identities.Authentication.Commands.ExternalLogin;

using AuthService.Application.Common.Messaging;
using AuthService.Application.Features.Identities.Authentication.Models.Responses;


/// <summary>
/// Command to confirm and create user from external login.
/// </summary>
/// <param name="Email">User's email address.</param>
/// <param name="FirstName">User's first name.</param>
/// <param name="LastName">User's last name.</param>
/// <param name="PhoneNumber">User's phone number (optional).</param>
/// <param name="Password">User's password for local login.</param>
/// <param name="IpAddress">Client IP address for token generation.</param>
/// <param name="Origin">Origin URL for confirmation emails.</param>
public sealed record ExternalLoginConfirmationCommand(
    string Email,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    string Password,
    string IpAddress,
    string Origin) : ICommand<TokenResponse>;
