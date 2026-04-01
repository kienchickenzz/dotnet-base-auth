/**
 * Contract for email sending service.
 *
 * <p>Defines the abstraction for sending emails.
 * Infrastructure layer provides concrete SMTP implementation.</p>
 */

namespace AuthService.Application.Common.ApplicationServices.Email;

/// <summary>
/// Email sending service contract.
/// </summary>
public interface IMailService
{
    /// <summary>
    /// Sends a single email asynchronously.
    /// </summary>
    Task SendAsync(IMailRequest request, CancellationToken cancellationToken = default);
}