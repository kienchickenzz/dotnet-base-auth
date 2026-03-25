/**
 * CreateUserCommand creates a new user.
 *
 * <p>Processed by CreateUserCommandHandler.</p>
 */
namespace AuthService.Application.Features.Identities.Users.Commands.CreateUser;

using AuthService.Application.Common.Messaging;


/// <summary>
/// Command to create a new user.
/// </summary>
/// <param name="FirstName">User's first name.</param>
/// <param name="LastName">User's last name.</param>
/// <param name="Email">User's email address.</param>
/// <param name="UserName">User's username.</param>
/// <param name="Password">User's password.</param>
/// <param name="ConfirmPassword">Password confirmation.</param>
/// <param name="PhoneNumber">User's phone number (optional).</param>
public sealed record CreateUserCommand(
    string FirstName,
    string LastName,
    string Email,
    string UserName,
    string Password,
    string ConfirmPassword,
    string? PhoneNumber) : ICommand<Guid>;
