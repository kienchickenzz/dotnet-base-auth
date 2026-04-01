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
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;

using AuthService.Application.Common.ApplicationServices.BackgroundJob;
using AuthService.Application.Common.ApplicationServices.Email;
using AuthService.Application.Common.ApplicationServices.Caching;
using AuthService.Application.Common.ApplicationServices.Serializer;
using AuthService.Infrastructure.Caching;
using AuthService.Infrastructure.Serializer;
using AuthService.Infrastructure.Settings;
using AuthService.Infrastructure.BackgroundJobs;

using AuthService.Infrastructure.Email;
using AuthService.Infrastructure.Email.Fake;
using AuthService.Infrastructure.Email.Mailkit;
using AuthService.Infrastructure.Email.Template;


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
            ._AddServices()
            ._AddBackgroundJobs(config)
            ._AddSerializer()
            ._AddMail(config);

        return services;
    }

    private static IServiceCollection _AddSettings(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<CacheSettings>(config.GetSection(CacheSettings.SectionName));
        services.Configure<HangfireSettings>(config.GetSection(HangfireSettings.SectionName));
        services.Configure<MailSettings>(config.GetSection(MailSettings.SectionName));

        return services;
    }

    private static IServiceCollection _AddServices(this IServiceCollection services)
    {
        services.AddScoped<IJobService, HangfireService>();
        

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

    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder builder)
    {
        builder._UseHangfireDashboard();

        return builder;
    }

    internal static IServiceCollection _AddBackgroundJobs(this IServiceCollection services, IConfiguration config)
    {
        services.AddHangfireServer();

        services.AddHangfire(hangfireConfig => hangfireConfig
            .UseSqlServerStorage(config.GetConnectionString("DefaultConnection"))
            .UseFilter(new LogJobFilter()));

        return services;
    }

    /// <summary>
    /// Configures Hangfire dashboard.
    /// </summary>
    private static IApplicationBuilder _UseHangfireDashboard(this IApplicationBuilder app)
    {
        var settings = app.ApplicationServices
            .GetRequiredService<IOptionsMonitor<HangfireSettings>>()
            .CurrentValue;

        var dashboardOptions = new DashboardOptions
        {
            AppPath = settings.Dashboard.AppPath,
            StatsPollingInterval = settings.Dashboard.StatsPollingInterval,
            DashboardTitle = settings.Dashboard.DashboardTitle,
            Authorization = new[]
            {
                new HangfireCustomBasicAuthenticationFilter
                {
                    User = settings.Credentials.User,
                    Pass = settings.Credentials.Password
                }
            }
        };

        return app.UseHangfireDashboard(settings.Route, dashboardOptions);
    }

    private static IServiceCollection _AddMail(this IServiceCollection services, IConfiguration config)
    {
        var settings = config
            .GetSection(MailSettings.SectionName)
            .Get<MailSettings>();

        services.AddSingleton<IMailRequestFactory, MailRequestFactory>();
        services.AddScoped<IEmailTemplateFactory, RazorEmailTemplateFactory>();

        // Register only the configured provider
        switch (settings?.Provider)
        {
            case EmailProviderEnum.Fake:
                services.AddScoped<IMailService, FakeMailService>();
                break;
            case EmailProviderEnum.MailKit:
            default:
                services.AddScoped<IMailService, SmtpMailService>();
                break;
        }

        return services;
    }
}
