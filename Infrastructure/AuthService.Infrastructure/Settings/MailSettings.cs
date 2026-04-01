/**
 * Mail configuration settings.
 *
 * <p>Contains provider selection and provider-specific configurations.
 * Designed for extensibility with multiple email providers.</p>
 */

namespace AuthService.Infrastructure.Settings;

using AuthService.Infrastructure.Email.Mailkit;


/// <summary>
/// Email provider options.
/// </summary>
public enum EmailProviderEnum
{
    /// <summary>
    /// Fake provider for development (log only, no actual sending).
    /// </summary>
    Fake,

    /// <summary>
    /// MailKit SMTP provider (production).
    /// </summary>
    MailKit
}

/// <summary>
/// Root mail configuration.
/// </summary>
public class MailSettings
{
    public const string SectionName = "MailSettings";

    /// <summary>
    /// Active email provider.
    /// </summary>
    public EmailProviderEnum Provider { get; set; } = EmailProviderEnum.MailKit;

    /// <summary>
    /// MailKit SMTP configuration.
    /// </summary>
    public MailkitOptions? Mailkit { get; set; }
}
