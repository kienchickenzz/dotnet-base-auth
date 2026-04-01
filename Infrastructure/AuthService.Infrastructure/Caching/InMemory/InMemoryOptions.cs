/**
 * InMemoryOptions contains configuration for in-memory cache.
 *
 * <p>Used when CacheSettings.Provider is InMemory.</p>
 */
namespace AuthService.Infrastructure.Caching.InMemory;


/// <summary>
/// Configuration options for in-memory cache provider.
/// </summary>
public sealed class InMemoryOptions
{
    /// <summary>
    /// Default sliding expiration time in minutes.
    /// </summary>
    public int DefaultSlidingExpirationMinutes { get; set; } = 10;
}
