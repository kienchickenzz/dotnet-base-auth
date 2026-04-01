/**
 * IHasDomainEvents defines contract for entities that can raise domain events.
 *
 * <p>Enables outbox pattern integration for any entity type.</p>
 */

namespace AuthService.Domain.Common;


/// <summary>
/// Interface for entities that can raise domain events.
/// </summary>
public interface IHasDomainEvents
{
    /// <summary>
    /// Gets all pending domain events.
    /// </summary>
    IReadOnlyList<IDomainEvent> GetDomainEvents();

    /// <summary>
    /// Clears all pending domain events.
    /// </summary>
    void ClearDomainEvents();
}
