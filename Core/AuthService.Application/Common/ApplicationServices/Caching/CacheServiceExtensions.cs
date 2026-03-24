/// <summary>
/// CacheServiceExtensions provides Cache-Aside (Lazy Loading) convenience methods for ICacheService.
///
/// <p>These extension methods encapsulate the common pattern of checking cache first,
/// falling back to a data source on cache miss, and populating cache before returning.
/// This avoids duplicating the cache-aside logic across the codebase.</p>
/// </summary>

namespace AuthService.Application.Common.ApplicationServices.Caching;

/// <summary>
/// Extension methods for <see cref="ICacheService"/> implementing the Cache-Aside pattern.
/// </summary>
public static class CacheServiceExtensions
{
    /// <summary>
    /// Gets a value from cache by key, or sets it using the provided callback on cache miss.
    /// </summary>
    /// <remarks>
    /// Uses Cache-Aside pattern: callers don't need to manage cache read/write logic manually,
    /// reducing boilerplate and ensuring consistent caching behavior across the application.
    /// </remarks>
    /// <typeparam name="T">The type of the cached value.</typeparam>
    /// <param name="cache">The cache service instance.</param>
    /// <param name="key">The cache key.</param>
    /// <param name="getItemCallback">Callback to fetch data from the source when cache misses.</param>
    /// <param name="slidingExpiration">Optional sliding expiration for the cache entry.</param>
    /// <returns>The cached or freshly fetched value, or null if the source returns null.</returns>
    public static T? GetOrSet<T>(this ICacheService cache, string key, Func<T?> getItemCallback, TimeSpan? slidingExpiration = null)
    {
        T? value = cache.Get<T>(key);

        if (value is not null)
        {
            return value;
        }

        value = getItemCallback();

        if (value is not null)
        {
            cache.Set(key, value, slidingExpiration);
        }

        return value;
    }

    /// <summary>
    /// Asynchronously gets a value from cache by key, or sets it using the provided callback on cache miss.
    /// </summary>
    /// <remarks>
    /// Async variant of <see cref="GetOrSet{T}"/> for I/O-bound data sources (e.g., database, external API).
    /// </remarks>
    /// <typeparam name="T">The type of the cached value.</typeparam>
    /// <param name="cache">The cache service instance.</param>
    /// <param name="key">The cache key.</param>
    /// <param name="getItemCallback">Async callback to fetch data from the source when cache misses.</param>
    /// <param name="slidingExpiration">Optional sliding expiration for the cache entry.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The cached or freshly fetched value, or null if the source returns null.</returns>
    public static async Task<T?> GetOrSetAsync<T>(this ICacheService cache, string key, Func<Task<T>> getItemCallback, TimeSpan? slidingExpiration = null, CancellationToken cancellationToken = default)
    {
        T? value = await cache.GetAsync<T>(key, cancellationToken);

        if (value is not null)
        {
            return value;
        }

        value = await getItemCallback();

        if (value is not null)
        {
            await cache.SetAsync(key, value, slidingExpiration, cancellationToken);
        }

        return value;
    }
}