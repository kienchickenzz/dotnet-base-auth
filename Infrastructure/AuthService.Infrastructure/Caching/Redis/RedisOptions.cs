/**
 * RedisOptions contains configuration for Redis cache.
 *
 * <p>Used when CacheSettings.Provider is Redis.</p>
 */
namespace AuthService.Infrastructure.Caching.Redis;


/// <summary>
/// Configuration options for Redis cache provider.
/// </summary>
public sealed class RedisOptions
{
    /// <summary>
    /// Redis connection string.
    /// </summary>
    public string Connection { get; set; } = string.Empty;

    /// <summary>
    /// Default sliding expiration time in minutes.
    /// </summary>
    public int DefaultSlidingExpirationMinutes { get; set; } = 10;
}
