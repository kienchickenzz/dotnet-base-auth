using AuthService.Application;
using AuthService.Infrastructure;
using AuthService.Persistence;
using AuthService.Identity.Initialization;
using AuthService.Identity;

using AuthService.Web.Extensions;
using AuthService.Web.Infrastructure.Mvc;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// MVC with Feature Folder pattern for Areas
builder.Services.AddFeatureFoldersMvc("AuthService.Web");

builder.Services.AddHttpContextAccessor();

builder.Host.UseSerilogFromSettings();

builder.Services.AddApplication();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddInfrastructurePersistence(builder.Configuration);
builder.Services.AddInfrastureIdentity(builder.Configuration);

var app = builder.Build();

// Initialize database (migrate + seed)
await app.Services.InitializeIdentityDatabaseAsync();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

// Identity middleware (JwtCookie + CurrentUser)
// Must run AFTER UseAuthentication to override context.User with JWT from cookie
app.UseInfrastructureIdentity();

app.UseAuthorization();

// Area routes: /customer/product/index -> Areas/Customer/Features/Product/Controllers/ProductController.Index()
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

// Default routes for non-area controllers
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Lifetime.ApplicationStarted.Register(() =>
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    var urls = string.Join(", ", app.Urls);
    logger.LogInformation("----------");
    logger.LogInformation("Application started successfully!");
    logger.LogInformation("Environment: {Environment}", app.Environment.EnvironmentName);
    logger.LogInformation("Listening on: {Urls}", urls);
    logger.LogInformation("----------");
});

app.Run();


public partial class Program { }
