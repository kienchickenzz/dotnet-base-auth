namespace AuthService.Infrastructure.Settings;


public enum CacheProvider
{
    InMemory,
    Redis
}

public class CacheSettings
{
    /// <summary>
    /// Section name in appsettings/configuration files.
    /// </summary>
    public const string SectionName = "CacheSettings";

    public CacheProvider Provider { get; set; } = CacheProvider.InMemory;
    public string? RedisConnection { get; set; }
    public int DefaultSlidingExpirationMinutes { get; set; } = 10;
}
