/**
 * ResetPasswordCommand resets user password with token.
 *
 * <p>Processed by ResetPasswordCommandHandler.</p>
 */
namespace AuthService.Application.Features.Identities.Users.Commands.Password;

using AuthService.Application.Common.Messaging;


/// <summary>
/// Command to reset user password.
/// </summary>
/// <param name="Email">User's email address.</param>
/// <param name="Token">Password reset token.</param>
/// <param name="NewPassword">New password.</param>
/// <param name="ConfirmNewPassword">New password confirmation.</param>
public sealed record ResetPasswordCommand(
    string Email,
    string Token,
    string NewPassword,
    string ConfirmNewPassword) : ICommand;
