/**
 * Configuration settings for Outbox pattern processing.
 *
 * <p>Maps to the "OutboxSettings" section in appsettings.json.</p>
 */

namespace AuthService.Persistence.Settings;


/// <summary>
/// Settings for outbox message processing job.
/// </summary>
public sealed class OutboxSettings
{
    public const string SectionName = "OutboxSettings";
    /// <summary>
    /// Interval in minutes between outbox processing runs (used for cron scheduling).
    /// </summary>
    public int IntervalInMinutes { get; init; } = 1;

    /// <summary>
    /// Number of outbox messages to process per batch.
    /// </summary>
    public int BatchSize { get; init; } = 10;
}
