/**
 * RoleUpdatedEvent is raised when a role is updated.
 *
 * <p>Used to invalidate permission caches for users with this role.</p>
 */
namespace AuthService.Domain.Events.Identity;

using AuthService.Domain.Common;


/// <summary>
/// Event raised when a role is updated.
/// </summary>
/// <param name="RoleId">The Id of the updated role.</param>
/// <param name="PermissionsUpdated">Whether permissions were updated.</param>
public sealed record RoleUpdatedEvent(
    Guid RoleId,
    bool PermissionsUpdated = false) : IDomainEvent;
