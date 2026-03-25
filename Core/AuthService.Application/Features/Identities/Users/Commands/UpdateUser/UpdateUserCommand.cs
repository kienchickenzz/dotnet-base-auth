/**
 * UpdateUserCommand updates an existing user profile.
 *
 * <p>Processed by UpdateUserCommandHandler.</p>
 */
namespace AuthService.Application.Features.Identities.Users.Commands.UpdateUser;

using AuthService.Application.Common.Messaging;


/// <summary>
/// Command to update user profile.
/// </summary>
/// <param name="Id">User's unique identifier.</param>
/// <param name="FirstName">User's first name.</param>
/// <param name="LastName">User's last name.</param>
/// <param name="Email">User's email address.</param>
/// <param name="PhoneNumber">User's phone number (optional).</param>
public sealed record UpdateUserCommand(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber) : ICommand;
