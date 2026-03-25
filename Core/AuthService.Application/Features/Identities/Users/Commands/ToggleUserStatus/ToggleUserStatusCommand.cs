/**
 * ToggleUserStatusCommand toggles user active status.
 *
 * <p>Processed by ToggleUserStatusCommandHandler.</p>
 */
namespace AuthService.Application.Features.Identities.Users.Commands.ToggleUserStatus;

using AuthService.Application.Common.Messaging;


/// <summary>
/// Command to toggle user active status.
/// </summary>
/// <param name="UserId">User's unique identifier.</param>
/// <param name="Activate">True to activate, false to deactivate.</param>
public sealed record ToggleUserStatusCommand(
    Guid UserId,
    bool Activate) : ICommand;
