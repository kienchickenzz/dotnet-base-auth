/**
 * ConfirmPhoneNumberCommand confirms user phone number.
 *
 * <p>Processed by ConfirmPhoneNumberCommandHandler.</p>
 */
namespace AuthService.Application.Features.Identities.Users.Commands.Confirm;

using AuthService.Application.Common.Messaging;


/// <summary>
/// Command to confirm user phone number.
/// </summary>
/// <param name="UserId">User's unique identifier.</param>
/// <param name="Code">Phone confirmation code.</param>
public sealed record ConfirmPhoneNumberCommand(
    Guid UserId,
    string Code) : ICommand<string>;
