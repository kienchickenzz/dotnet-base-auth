/**
 * UserStatusChangedEvent is raised when user status changes.
 *
 * <p>Allows other parts of the system to react to status changes.</p>
 */
namespace AuthService.Domain.Events.Identity;

using AuthService.Domain.Common;


/// <summary>
/// Event raised when user status changes.
/// </summary>
/// <param name="UserId">The Id of the user.</param>
/// <param name="IsActive">The new active status.</param>
public sealed record UserStatusChangedEvent(
    Guid UserId,
    bool IsActive) : IDomainEvent;
