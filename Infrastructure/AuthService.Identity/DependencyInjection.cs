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
using AuthService.Application.Features.Identities.Authentication;
using AuthService.Application.Features.Identities.Authentication.Services;
using AuthService.Identity.Auth;
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
    public static IServiceCollection AddInfrastureIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<ApplicationDbInitializer>();
        services.AddTransient<ApplicationDbSeeder>();

        // Authentication services (CQRS pattern)
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();

        // New Identity abstractions (Application layer interfaces)
        services.AddScoped<IIdentityUserService, IdentityUserService>();
        services.AddScoped<IIdentityRoleService, IdentityRoleService>();
        services.AddScoped<IIdentityPermissionService, IdentityPermissionService>();
        services.AddScoped<IIdentityAuthService, IdentityAuthService>();

        services.Configure<AdminSetting>(configuration.GetSection("SecuritySettings:AdminSettings"));

        services
            ._AddIdentity(configuration)
            ._AddJwtAuth(configuration)
            ._AddCurrentUser()
            ._AddPermissions()
            ._AddOutbox(configuration);

        return services;
    }

    public static IApplicationBuilder UseInfrastructureIdentity(this IApplicationBuilder builder) =>
        builder
            ._UseCurrentUser();

    // TODO: hàm không thật sự đăng ký DI mà seed dữ liệu, nên đặt chỗ khác??
    public static async Task InitializeApplicationIdentityDatabaseAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
    {
        // Create a new scope to retrieve scoped services
        using var scope = services.CreateScope();

        await scope.ServiceProvider.GetRequiredService<ApplicationDbInitializer>()
            .InitializeAsync(cancellationToken);
    }

    // TODO: Đổi tên hàm này để phản ánh đúng hơn chức năng của nó
    private static IApplicationBuilder _UseCurrentUser(this IApplicationBuilder app) =>
        app.UseMiddleware<CurrentUserMiddleware>();

    /// <summary>
    /// Configures ASP.NET Core Identity with custom stores and Unit of Work pattern.
    /// </summary>
    private static IServiceCollection _AddIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationIdentityDbContext>(options =>
               options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
                .AddDefaultTokenProviders();

        // Configure UserStore with AutoSaveChanges = false
        // Transaction is managed by TransactionPipelineBehavior
        // Must specify all type parameters to match ApplicationIdentityDbContext
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
        // Must specify ApplicationRoleClaim instead of default IdentityRoleClaim<Guid>
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

        // Register IIdentityUnitOfWork for transaction management
        services.AddScoped<IIdentityUnitOfWork>(sp =>
            sp.GetRequiredService<ApplicationIdentityDbContext>());

        services.AddHttpContextAccessor();

        return services;
    }
        
    // TODO: Ý nghĩa hàm này là gì?
    private static IServiceCollection _AddCurrentUser(this IServiceCollection services) =>
        services
            .AddScoped<CurrentUserMiddleware>()
            .AddScoped<ICurrentUser, CurrentUser>()
            .AddScoped(sp => (ICurrentUserInitializer) sp.GetRequiredService<ICurrentUser>());

    private static IServiceCollection _AddPermissions(this IServiceCollection services) =>
        services
            .AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>()
            .AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

    private static IServiceCollection _AddJwtAuth(this IServiceCollection services, IConfiguration configuration)
    {
        // TODO: Dùng cách khác để load JwtSettings
        services.AddOptions<JwtSettings>()
            .BindConfiguration($"SecuritySettings:{nameof(JwtSettings)}")
            .ValidateDataAnnotations()
            .ValidateOnStart();

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
    /// Configures outbox pattern for Identity bounded context.
    /// </summary>
    private static IServiceCollection _AddOutbox(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddKeyedSingleton<ISqlConnectionFactory>(
            "Identity",
            (_, _) => new SqlConnectionFactory(connectionString));

        services.Configure<OutboxSettings>(configuration.GetSection("IdentityOutboxSettings"));

        return services;
    }

    /// <summary>
    /// Registers recurring job to process Identity outbox messages.
    /// </summary>
    public static void AddIdentityOutboxJob(this IServiceProvider serviceProvider, IConfiguration configuration)
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
