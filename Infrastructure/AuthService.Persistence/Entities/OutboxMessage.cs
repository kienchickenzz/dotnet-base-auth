/**
 * OutboxMessage represents a domain event stored for reliable publishing.
 *
 * <p>Part of the Outbox Pattern - events are saved atomically with business data,
 * then processed asynchronously by a background job to ensure at-least-once delivery.</p>
 */

namespace AuthService.Persistence.Entities;


/// <summary>
/// Entity storing domain events for deferred publishing.
/// </summary>
public sealed class OutboxMessage
{
    public OutboxMessage(Guid id, DateTime occurredOnUtc, string type, string content)
    {
        Id = id;
        OccurredOnUtc = occurredOnUtc;
        Type = type;
        Content = content;
    }

    public Guid Id { get; init; }

    public DateTime OccurredOnUtc { get; init; }

    public string Type { get; init; }

    public string Content { get; init; }

    public DateTime? ProcessedOnUtc { get; init; }

    public string? Error { get; init; }
}
