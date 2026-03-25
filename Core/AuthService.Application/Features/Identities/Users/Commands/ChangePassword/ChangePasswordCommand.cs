/**
 * ChangePasswordCommand changes user password.
 *
 * <p>Processed by ChangePasswordCommandHandler.</p>
 */
namespace AuthService.Application.Features.Identities.Users.Commands.Password;

using AuthService.Application.Common.Messaging;


/// <summary>
/// Command to change user password.
/// </summary>
/// <param name="UserId">User's unique identifier.</param>
/// <param name="CurrentPassword">User's current password.</param>
/// <param name="NewPassword">New password.</param>
/// <param name="ConfirmNewPassword">New password confirmation.</param>
public sealed record ChangePasswordCommand(
    Guid UserId,
    string CurrentPassword,
    string NewPassword,
    string ConfirmNewPassword) : ICommand;
