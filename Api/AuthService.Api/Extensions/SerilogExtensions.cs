/**
 * SerilogExtensions provides extension methods for configuring Serilog.
 *
 * <p>This extension encapsulates Serilog configuration logic,
 * reading settings from a configurable section in appsettings/config files.</p>
 */
namespace AuthService.Api.Extensions;

using Serilog;

/// <summary>
/// Extension methods for configuring Serilog logging.
/// </summary>
public static class SerilogExtensions
{
    /// <summary>
    /// Default configuration section name for logger settings.
    /// </summary>
    private const string DefaultSectionName = "LoggerSettings";

    /// <summary>
    /// Configures Serilog using settings from the specified configuration section.
    /// </summary>
    /// <param name="host">The host builder instance.</param>
    /// <param name="sectionName">
    /// The configuration section name containing Serilog settings.
    /// Defaults to "LoggerSettings".
    /// </param>
    /// <returns>The configured host builder for chaining.</returns>
    public static IHostBuilder UseSerilogFromSettings(this IHostBuilder host, string sectionName = DefaultSectionName)
    {
        return host.UseSerilog((context, config) =>
        {
            var section = context.Configuration.GetSection(sectionName);
            config.ReadFrom.Configuration(section);
        });
    }
}
