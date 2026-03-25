/**
 * UserUpdatedEvent is raised when a user is updated.
 *
 * <p>Allows other parts of the system to react to user updates.</p>
 */
namespace AuthService.Domain.Events.Identity;

using AuthService.Domain.Common;


/// <summary>
/// Event raised when a user is updated.
/// </summary>
/// <param name="UserId">The Id of the updated user.</param>
/// <param name="RolesUpdated">Whether roles were updated.</param>
public sealed record UserUpdatedEvent(
    Guid UserId,
    bool RolesUpdated = false) : IDomainEvent;
