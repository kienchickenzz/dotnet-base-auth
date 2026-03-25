/**
 * ConfirmEmailCommand confirms user email address.
 *
 * <p>Processed by ConfirmEmailCommandHandler.</p>
 */
namespace AuthService.Application.Features.Identities.Users.Commands.Confirm;

using AuthService.Application.Common.Messaging;


/// <summary>
/// Command to confirm user email.
/// </summary>
/// <param name="UserId">User's unique identifier.</param>
/// <param name="Code">Email confirmation code.</param>
public sealed record ConfirmEmailCommand(
    Guid UserId,
    string Code) : ICommand<string>;
