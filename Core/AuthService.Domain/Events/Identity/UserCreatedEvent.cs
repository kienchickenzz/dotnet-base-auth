/**
 * UserCreatedEvent is raised when a user is created.
 *
 * <p>Allows other parts of the system to react to user creation.</p>
 */
namespace AuthService.Domain.Events.Identity;

using AuthService.Domain.Common;


/// <summary>
/// Event raised when a user is created.
/// </summary>
/// <param name="UserId">The Id of the created user.</param>
public sealed record UserCreatedEvent(Guid UserId) : IDomainEvent;
