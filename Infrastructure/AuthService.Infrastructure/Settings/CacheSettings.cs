/**
 * CacheSettings contains configuration for caching.
 *
 * <p>Supports InMemory and Redis cache providers.</p>
 */
namespace AuthService.Infrastructure.Settings;

using AuthService.Infrastructure.Caching.InMemory;
using AuthService.Infrastructure.Caching.Redis;


/// <summary>
/// Cache provider types.
/// </summary>
public enum CacheProvider
{
    InMemory,
    Redis
}

/// <summary>
/// Configuration settings for caching.
/// </summary>
public class CacheSettings
{
    /// <summary>
    /// Section name in appsettings/configuration files.
    /// </summary>
    public const string SectionName = "CacheSettings";

    /// <summary>
    /// Cache provider to use.
    /// </summary>
    public CacheProvider Provider { get; set; } = CacheProvider.InMemory;

    /// <summary>
    /// InMemory cache configuration. Used when Provider is InMemory.
    /// </summary>
    public InMemoryOptions? InMemory { get; set; }

    /// <summary>
    /// Redis cache configuration. Used when Provider is Redis.
    /// </summary>
    public RedisOptions? Redis { get; set; }
}
