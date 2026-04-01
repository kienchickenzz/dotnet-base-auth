/**
 * Fake email service for development.
 *
 * <p>Logs email content instead of sending.
 * Useful for local development without SMTP setup.</p>
 */

namespace AuthService.Infrastructure.Email.Fake;

using Microsoft.Extensions.Logging;

using AuthService.Application.Common.ApplicationServices.Email;

/// <summary>
/// Fake mail service that only logs emails.
/// </summary>
public class FakeMailService : IMailService
{
    private readonly ILogger<FakeMailService> _logger;

    public FakeMailService(ILogger<FakeMailService> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public Task SendAsync(IMailRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "[FAKE EMAIL] To: {To}, Subject: {Subject}, Body Length: {BodyLength}",
            request.To,
            request.Subject,
            request.Body?.Length ?? 0);

        return Task.CompletedTask;
    }
}
