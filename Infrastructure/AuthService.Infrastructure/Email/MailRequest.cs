/**
 * Mail request models and factory.
 *
 * <p>Contains MailRequest, Attachment implementations and their factory.</p>
 */

namespace AuthService.Infrastructure.Email;

using AuthService.Application.Common.ApplicationServices.Email;

/// <summary>
/// Email attachment implementation.
/// </summary>
public class Attachment : IAttachment
{
    /// <summary>
    /// Creates a new attachment.
    /// </summary>
    public Attachment(string fileName, byte[] content)
    {
        FileName = fileName;
        Content = content;
    }

    /// <inheritdoc />
    public string FileName { get; }

    /// <inheritdoc />
    public byte[] Content { get; }
}

/// <summary>
/// Email request implementation.
/// One instance = one email to one recipient.
/// </summary>
public class MailRequest : IMailRequest
{
    /// <summary>
    /// Creates a new mail request.
    /// </summary>
    public MailRequest(
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
        IReadOnlyDictionary<string, string>? headers = null)
    {
        To = to;
        Subject = subject;
        Body = body;
        From = from;
        DisplayName = displayName;
        ReplyTo = replyTo;
        ReplyToName = replyToName;
        Bcc = bcc;
        Cc = cc;
        Attachments = attachments ?? Array.Empty<IAttachment>();
        Headers = headers ?? new Dictionary<string, string>();
    }

    /// <inheritdoc />
    public string To { get; }

    /// <inheritdoc />
    public string Subject { get; }

    /// <inheritdoc />
    public string? Body { get; }

    /// <inheritdoc />
    public string? From { get; }

    /// <inheritdoc />
    public string? DisplayName { get; }

    /// <inheritdoc />
    public string? ReplyTo { get; }

    /// <inheritdoc />
    public string? ReplyToName { get; }

    /// <inheritdoc />
    public string? Bcc { get; }

    /// <inheritdoc />
    public string? Cc { get; }

    /// <inheritdoc />
    public IReadOnlyList<IAttachment> Attachments { get; }

    /// <inheritdoc />
    public IReadOnlyDictionary<string, string> Headers { get; }
}

/// <summary>
/// Factory for creating mail requests and attachments.
/// </summary>
public class MailRequestFactory : IMailRequestFactory
{
    /// <inheritdoc />
    public IMailRequest Create(
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
        IReadOnlyDictionary<string, string>? headers = null)
    {
        return new MailRequest(
            to, subject, body, from, displayName,
            replyTo, replyToName, bcc, cc,
            attachments, headers);
    }

    /// <inheritdoc />
    public IAttachment CreateAttachment(string fileName, byte[] content)
    {
        return new Attachment(fileName, content);
    }
}
