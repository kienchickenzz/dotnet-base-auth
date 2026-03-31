/**
 * DeleteUserCommand soft-deletes a user account.
 *
 * <p>Marks user as deleted via ISoftDelete fields.</p>
 */
namespace AuthService.Application.Features.Identities.Users.Commands.DeleteUser;

using AuthService.Application.Common.Messaging;


/// <summary>
/// Command to soft-delete a user.
/// </summary>
public sealed record DeleteUserCommand(Guid UserId) : ICommand;
