/**
 * UserRolesUpdatedEvent is raised when user roles are updated.
 *
 * <p>Used to invalidate permission cache and notify other services.</p>
 */
namespace AuthService.Domain.Events.Identity;

using AuthService.Domain.Common;


/// <summary>
/// Event raised when user roles are updated.
/// </summary>
/// <param name="UserId">The Id of the user.</param>
public sealed record UserRolesUpdatedEvent(Guid UserId) : IDomainEvent;
