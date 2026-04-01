/**
 * Factory contract for creating mail requests.
 *
 * <p>Abstracts the creation of IMailRequest instances.
 * Allows Application layer to create mail requests without
 * depending on Infrastructure implementation.</p>
 */

namespace AuthService.Application.Common.ApplicationServices.Email;

/// <summary>
/// Factory for creating mail request instances.
/// </summary>
public interface IMailRequestFactory
{
    /// <summary>
    /// Creates a mail request.
    /// </summary>
    IMailRequest Create(
        string to,
        string subject,
        string? body = null,
        string? from = null,
        string? displayName = null,
        string? replyTo = null,
        string? replyToName = null,
        string? bcc = null,
        string? cc = null,
        IReadOnlyList<IAttachment>? attachments = null,
        IReadOnlyDictionary<string, string>? headers = null);

    /// <summary>
    /// Creates an attachment.
    /// </summary>
    IAttachment CreateAttachment(string fileName, byte[] content);
}
