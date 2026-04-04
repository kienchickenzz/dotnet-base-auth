namespace AuthService.Identity;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using AuthService.Application.Common.Abstractions.Identity;
using AuthService.Application.Common.ApplicationServices.Auth;
using AuthService.Application.Common.ApplicationServices.BackgroundJob;
using AuthService.Application.Common.ApplicationServices.Persistence;
using AuthService.Application.Features.Identities.Authentication.Services;
using AuthService.Identity.Auth.Jwt;
using AuthService.Identity.Auth.Permissions;
using AuthService.Identity.DatabaseContext;
using AuthService.Identity.Entities;
using AuthService.Identity.Initialization;
using AuthService.Identity.Middlewares;
using AuthService.Identity.Outbox;
using AuthService.Identity.Services;
using AuthService.Identity.Settings;


public static class DependencyInjection
{
    /// <summary>
    /// Registers all Identity infrastructure services.
    /// </summary>
    public static IServiceCollection AddInfrastureIdentity(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddTransient<ApplicationDbSeeder>();

        services
            ._AddDatabase(configuration)
            ._AddSettings(configuration)
            ._AddIdentity()
            ._AddJwtAuth()
            ._AddServices()
            ._AddMiddlewares()
            ._AddCurrentUserContext()
            ._AddPermissions();

        return services;
    }

    /// <summary>
    /// Configures Identity middleware pipeline.
    /// </summary>
    public static IApplicationBuilder UseInfrastructureIdentity(this IApplicationBuilder builder) =>
        builder
            .UseMiddleware<JwtCookieMiddleware>()
            .UseMiddleware<CurrentUserMiddleware>();

    /// <summary>
    /// Configures DbContext, UnitOfWork, and SQL connection factory.
    /// </summary>
    private static IServiceCollection _AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new ArgumentNullException(nameof(configuration));

        services.AddDbContext<ApplicationIdentityDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IIdentityUnitOfWork>(sp =>
            sp.GetRequiredService<ApplicationIdentityDbContext>());

        services.AddKeyedSingleton<ISqlConnectionFactory>(
            "Identity",
            (_, _) => new SqlConnectionFactory(connectionString));

        return services;
    }

    /// <summary>
    /// Registers configuration settings with IOptions pattern.
    /// </summary>
    private static IServiceCollection _AddSettings(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.Configure<AdminSettings>(config.GetSection(AdminSettings.SectionName));
        services.Configure<OutboxSettings>(config.GetSection(OutboxSettings.SectionName));
        services.Configure<JwtSettings>(config.GetSection(JwtSettings.SectionName));

        return services;
    }

    /// <summary>
    /// Configures ASP.NET Core Identity with custom stores.
    /// </summary>
    private static IServiceCollection _AddIdentity(this IServiceCollection services)
    {
        services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
                .AddDefaultTokenProviders();

        // Configure UserStore with AutoSaveChanges = false
        services.AddScoped<IUserStore<ApplicationUser>>(sp =>
        {
            var context = sp.GetRequiredService<ApplicationIdentityDbContext>();
            return new UserStore<
                ApplicationUser,
                ApplicationRole,
                ApplicationIdentityDbContext,
                Guid,
                IdentityUserClaim<Guid>,
                IdentityUserRole<Guid>,
                IdentityUserLogin<Guid>,
                IdentityUserToken<Guid>,
                ApplicationRoleClaim>(context)
            {
                AutoSaveChanges = false
            };
        });

        // Configure RoleStore with AutoSaveChanges = false
        services.AddScoped<IRoleStore<ApplicationRole>>(sp =>
        {
            var context = sp.GetRequiredService<ApplicationIdentityDbContext>();
            return new RoleStore<
                ApplicationRole,
                ApplicationIdentityDbContext,
                Guid,
                IdentityUserRole<Guid>,
                ApplicationRoleClaim>(context)
            {
                AutoSaveChanges = false
            };
        });

        return services;
    }

    /// <summary>
    /// Configures JWT Bearer authentication.
    /// </summary>
    private static IServiceCollection _AddJwtAuth(this IServiceCollection services)
    {
        services.AddSingleton<IConfigureOptions<JwtBearerOptions>, ConfigureJwtBearerOptions>();
        services.AddTransient<TokenValidatedJwtBearerEvents>();

        services
            .AddAuthentication(authentication =>
            {
                authentication.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                authentication.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, null!);

        return services;
    }

    /// <summary>
    /// Registers authentication and identity application services.
    /// </summary>
    private static IServiceCollection _AddServices(this IServiceCollection services)
    {
        // Authentication services (CQRS pattern)
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<ISignInService, SignInService>();

        // Identity abstractions (Application layer interfaces)
        services.AddScoped<IIdentityUserService, IdentityUserService>();
        services.AddScoped<IIdentityRoleService, IdentityRoleService>();
        services.AddScoped<IIdentityPermissionService, IdentityPermissionService>();
        services.AddScoped<IIdentityAuthService, IdentityAuthService>();

        return services;
    }

    /// <summary>
    /// Registers middleware services (factory-based IMiddleware).
    /// </summary>
    private static IServiceCollection _AddMiddlewares(this IServiceCollection services)
    {
        services.AddScoped<JwtCookieMiddleware>();
        services.AddScoped<CurrentUserMiddleware>();

        return services;
    }

    /// <summary>
    /// Registers ICurrentUser and ICurrentUserInitializer for request-scoped user context.
    /// </summary>
    private static IServiceCollection _AddCurrentUserContext(this IServiceCollection services) =>
        services
            .AddScoped<ICurrentUser, CurrentUser>()
            .AddScoped(sp => (ICurrentUserInitializer)sp.GetRequiredService<ICurrentUser>());

    /// <summary>
    /// Registers permission-based authorization handlers.
    /// </summary>
    private static IServiceCollection _AddPermissions(this IServiceCollection services) =>
        services
            .AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>()
            .AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

    /// <summary>
    /// Registers recurring job to process Identity outbox messages.
    /// </summary>
    public static void AddIdentityOutboxJob(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var job = scope.ServiceProvider.GetRequiredService<IJobService>();
        var settings = scope.ServiceProvider
            .GetRequiredService<IOptions<OutboxSettings>>()
            .Value;

        job.Recurring<ProcessOutboxMessagesJob>(
            "ProcessIdentityOutboxMessages",
            j => j.Execute(),
            $"*/{settings.IntervalInMinutes} * * * *");
    }
}
