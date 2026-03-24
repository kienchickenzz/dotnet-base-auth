/// <summary>
/// DependencyInjection registers all Infrastructure-layer services into the DI container.
///
/// <para>Centralizes service registration (caching, serialization) to keep Program.cs clean
/// and ensure Infrastructure concerns are configured in one place.</para>
/// </summary>

namespace AuthService.Infrastructure;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

using AuthService.Application.Common.ApplicationServices.Caching;
using AuthService.Application.Common.ApplicationServices.Serializer;
using AuthService.Infrastructure.Caching;
using AuthService.Infrastructure.Serializer;
using AuthService.Infrastructure.Settings;


/// <summary>
/// Extension methods for registering Infrastructure services into <see cref="IServiceCollection"/>.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Đăng ký toàn bộ service của tầng Infrastructure (caching, serializer).
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services
            ._AddSettings(config)
            ._AddCaching(config)
            ._AddSerializer();

        return services;
    }

    private static IServiceCollection _AddSettings(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<CacheSettings>(config.GetSection(CacheSettings.SectionName));

        return services;
    }

    /// <summary>
    /// Đăng ký cache provider dựa trên cấu hình.
    /// Ưu tiên Redis nếu được cấu hình và kết nối thành công, ngược lại fallback về InMemory.
    /// Logging được thực hiện trong constructor của từng service (construction logging pattern).
    /// </summary>
    private static IServiceCollection _AddCaching(this IServiceCollection services, IConfiguration config)
    {
        var settings = config
            .GetSection(CacheSettings.SectionName)
            .Get<CacheSettings>();

        services.AddSingleton<ICacheKeyService, CacheKeyService>();

        var useRedis = settings?.Provider == CacheProvider.Redis
            && _TryConnectRedis(settings.RedisConnection);

        if (useRedis)
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = settings!.RedisConnection;
            });
            services.AddSingleton<ICacheService, RedisCacheService>();
        }
        else
        {
            services.AddMemoryCache();
            services.AddSingleton<ICacheService, InMemoryCacheService>();
        }

        return services;
    }

    /// <summary>
    /// Kiểm tra kết nối Redis có hoạt động không.
    /// </summary>
    private static bool _TryConnectRedis(string? connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            return false;

        try
        {
            using var connection = ConnectionMultiplexer.Connect(
                connectionString,
                options => options.ConnectTimeout = 5000);

            return connection.IsConnected;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Đăng ký serializer service (singleton vì stateless).
    /// </summary>
    private static IServiceCollection _AddSerializer(this IServiceCollection services)
    {
        services.AddSingleton<ISerializerService, NewtonSoftService>();
        return services;
    }
}
