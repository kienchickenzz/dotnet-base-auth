/// <summary>
/// InMemoryCacheService provides in-process caching using IMemoryCache.
///
/// <para>Used as default/fallback cache when Redis is unavailable or not configured.</para>
/// </summary>

namespace AuthService.Infrastructure.Caching;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using AuthService.Application.Common.ApplicationServices.Caching;
using AuthService.Infrastructure.Settings;


/// <summary>
/// In-memory cache implementation using Microsoft.Extensions.Caching.Memory.
/// </summary>
public class InMemoryCacheService : ICacheService
{
    private readonly IMemoryCache _cache;
    private readonly CacheSettings _settings;
    private readonly ILogger<InMemoryCacheService> _logger;

    /// <summary>
    /// Initializes InMemoryCacheService with required dependencies.
    /// Logs initialization info (construction logging pattern).
    /// </summary>
    public InMemoryCacheService(
        IMemoryCache cache,
        IOptions<CacheSettings> settings,
        ILogger<InMemoryCacheService> logger)
    {
        _cache = cache;
        _settings = settings.Value;
        _logger = logger;

        _logger.LogInformation(
            "Cache initialized: InMemory (DefaultSlidingExpiration={Minutes}m)",
            _DefaultSlidingExpirationMinutes);
    }

    /// <summary>
    /// Gets the default sliding expiration in minutes from settings.
    /// </summary>
    private int _DefaultSlidingExpirationMinutes =>
        _settings.InMemory?.DefaultSlidingExpirationMinutes ?? 10;

    public T? Get<T>(string key) =>
        _cache.Get<T>(key);

    public Task<T?> GetAsync<T>(string key, CancellationToken token = default) =>
        Task.FromResult(Get<T>(key));

    public void Refresh(string key) =>
        _cache.TryGetValue(key, out _);

    public Task RefreshAsync(string key, CancellationToken token = default)
    {
        Refresh(key);
        return Task.CompletedTask;
    }

    public void Remove(string key) =>
        _cache.Remove(key);

    public Task RemoveAsync(string key, CancellationToken token = default)
    {
        Remove(key);
        return Task.CompletedTask;
    }

    public void Set<T>(string key, T value, TimeSpan? slidingExpiration = null)
    {
        slidingExpiration ??= TimeSpan.FromMinutes(_DefaultSlidingExpirationMinutes);

        _cache.Set(key, value, new MemoryCacheEntryOptions
        {
            SlidingExpiration = slidingExpiration
        });

        _logger.LogDebug("Added to InMemory Cache: {Key}", key);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? slidingExpiration = null, CancellationToken token = default)
    {
        Set(key, value, slidingExpiration);
        return Task.CompletedTask;
    }
}
