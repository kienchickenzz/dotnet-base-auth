/**
 * MailKit SMTP provider configuration.
 *
 * <p>Contains SMTP connection settings for MailKit email sending.</p>
 */

namespace AuthService.Infrastructure.Email.Mailkit;


/// <summary>
/// MailKit SMTP provider configuration.
/// </summary>
public class MailkitOptions
{
    /// <summary>
    /// Default sender email address.
    /// </summary>
    public string? From { get; set; }

    /// <summary>
    /// SMTP server hostname.
    /// </summary>
    public string? Host { get; set; }

    /// <summary>
    /// SMTP server port.
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// SMTP authentication username.
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// SMTP authentication password.
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Default sender display name.
    /// </summary>
    public string? DisplayName { get; set; }
}
