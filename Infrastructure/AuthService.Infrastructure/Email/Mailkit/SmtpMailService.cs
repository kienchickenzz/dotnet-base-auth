/**
 * SMTP implementation of IMailService.
 *
 * <p>Sends emails using MailKit SMTP client.
 * Supports Gmail with App Password authentication.</p>
 */

namespace AuthService.Infrastructure.Email.Mailkit;

using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

using AuthService.Application.Common.ApplicationServices.Email;
using AuthService.Infrastructure.Settings;

/// <summary>
/// SMTP-based email sending service.
/// </summary>
public class SmtpMailService : IMailService
{
    private readonly MailkitOptions _settings;
    private readonly ILogger<SmtpMailService> _logger;

    public SmtpMailService(IOptions<MailSettings> settings, ILogger<SmtpMailService> logger)
    {
        _settings = settings.Value.Mailkit
            ?? throw new InvalidOperationException("Mailkit settings not configured");
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task SendAsync(IMailRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var email = new MimeMessage();

            // From
            email.From.Add(new MailboxAddress(_settings.DisplayName, request.From ?? _settings.From));

            // To (single recipient)
            email.To.Add(MailboxAddress.Parse(request.To));

            // Reply To
            if (!string.IsNullOrEmpty(request.ReplyTo))
                email.ReplyTo.Add(new MailboxAddress(request.ReplyToName, request.ReplyTo));

            // Bcc
            if (!string.IsNullOrWhiteSpace(request.Bcc))
                email.Bcc.Add(MailboxAddress.Parse(request.Bcc.Trim()));

            // Cc
            if (!string.IsNullOrWhiteSpace(request.Cc))
                email.Cc.Add(MailboxAddress.Parse(request.Cc.Trim()));

            // Headers
            foreach (var header in request.Headers)
                email.Headers.Add(header.Key, header.Value);

            // Content
            var builder = new BodyBuilder();
            email.Sender = new MailboxAddress(request.DisplayName ?? _settings.DisplayName, request.From ?? _settings.From);
            email.Subject = request.Subject;
            builder.HtmlBody = request.Body;

            // Attachments
            foreach (var attachment in request.Attachments)
                builder.Attachments.Add(attachment.FileName, attachment.Content);

            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls, cancellationToken);
            await smtp.AuthenticateAsync(_settings.UserName, _settings.Password, cancellationToken);
            await smtp.SendAsync(email, cancellationToken);
            await smtp.DisconnectAsync(true, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }
    }
}
