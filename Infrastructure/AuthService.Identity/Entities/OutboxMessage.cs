/**
 * OutboxMessage represents a domain event stored for reliable publishing.
 *
 * <p>Part of the Outbox Pattern for Identity bounded context.
 * Events are saved atomically with user data, then processed asynchronously.</p>
 */

namespace AuthService.Identity.Entities;


/// <summary>
/// Entity storing domain events for deferred publishing in Identity context.
/// </summary>
public sealed class OutboxMessage
{
    /// <summary>
    /// Default constructor for EF Core.
    /// </summary>
    public OutboxMessage() { }

    /// <summary>
    /// Creates a new outbox message.
    /// </summary>
    public OutboxMessage(Guid id, DateTime occurredOnUtc, string type, string content)
    {
        Id = id;
        OccurredOnUtc = occurredOnUtc;
        Type = type;
        Content = content;
    }

    /// <summary>
    /// Unique identifier.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// When the event occurred.
    /// </summary>
    public DateTime OccurredOnUtc { get; init; }

    /// <summary>
    /// Full type name for deserialization.
    /// </summary>
    public string Type { get; init; } = string.Empty;

    /// <summary>
    /// JSON-serialized domain event content.
    /// </summary>
    public string Content { get; init; } = string.Empty;

    /// <summary>
    /// When the message was processed (null if pending).
    /// </summary>
    public DateTime? ProcessedOnUtc { get; set; }

    /// <summary>
    /// Error message if processing failed.
    /// </summary>
    public string? Error { get; set; }
}
