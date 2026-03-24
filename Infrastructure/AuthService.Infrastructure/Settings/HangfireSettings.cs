/**
 * HangfireSettings maps configuration from hangfire.json.
 *
 * <p>Used with IOptions pattern for strongly-typed configuration access.</p>
 */

namespace AuthService.Infrastructure.Settings;


/// <summary>
/// Root configuration for Hangfire background job processing.
/// </summary>
public sealed class HangfireSettings
{
    /// <summary>
    /// Section name in appsettings/configuration files.
    /// </summary>
    public const string SectionName = "HangfireSettings";

    /// <summary>
    /// URL route for Hangfire dashboard (e.g., "/jobs").
    /// </summary>
    public string Route { get; set; } = "/jobs";

    /// <summary>
    /// Dashboard UI configuration options.
    /// </summary>
    public HangfireDashboardSettings Dashboard { get; set; } = new();

    /// <summary>
    /// Authentication credentials for dashboard access.
    /// </summary>
    public HangfireCredentials Credentials { get; set; } = new();
}

/// <summary>
/// Dashboard UI specific settings.
/// </summary>
public sealed class HangfireDashboardSettings
{
    /// <summary>
    /// Base application path for dashboard links.
    /// </summary>
    public string AppPath { get; set; } = "/";

    /// <summary>
    /// Interval in milliseconds for polling job statistics.
    /// </summary>
    public int StatsPollingInterval { get; set; } = 2000;

    /// <summary>
    /// Title displayed on dashboard header.
    /// </summary>
    public string DashboardTitle { get; set; } = "Jobs";
}

/// <summary>
/// Basic authentication credentials for dashboard.
/// </summary>
public sealed class HangfireCredentials
{
    /// <summary>
    /// Username for dashboard login.
    /// </summary>
    public string User { get; set; } = string.Empty;

    /// <summary>
    /// Password for dashboard login.
    /// </summary>
    public string Password { get; set; } = string.Empty;
}
