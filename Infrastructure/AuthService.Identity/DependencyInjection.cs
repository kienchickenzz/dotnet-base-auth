namespace AuthService.Identity;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using AuthService.Application.Common.Abstractions.Identity;
using AuthService.Application.Common.ApplicationServices.Auth;
using AuthService.Application.Features.Identities.Authentication;
using AuthService.Application.Features.Identities.Authentication.Services;
using AuthService.Identity.Auth;
using AuthService.Identity.Auth.Jwt;
using AuthService.Identity.Auth.Permissions;
using AuthService.Identity.DatabaseContext;
using AuthService.Identity.Entities;
using AuthService.Identity.Initialization;
using AuthService.Identity.Middlewares;
using AuthService.Identity.Services;


public static class DependencyInjection
{
    public static IServiceCollection AddInfrastureIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<ApplicationDbInitializer>();
        services.AddTransient<ApplicationDbSeeder>();

        // Legacy services (to be deprecated)
        // TODO: Thay ITokenService bằng IJwtTokenGenerator và IAuthenticationService, sau đó xóa ITokenService
        services.AddScoped<ITokenService, TokenService>();

        // New CQRS services (Authentication layer)
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
            ._AddPermissions();

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

    private static IServiceCollection _AddIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
                .AddDefaultTokenProviders();

        services.AddDbContext<ApplicationIdentityDbContext>(options =>
               options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Dòng này thừa??
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
}
