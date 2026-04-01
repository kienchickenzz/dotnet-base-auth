/**
 * Contract for email request.
 *
 * <p>Defines the structure for sending a single email.
 * Each MailRequest instance represents one email to one recipient.
 * Read-only interface ensuring immutability at the contract level.
 * Infrastructure layer provides concrete implementation.</p>
 */

namespace AuthService.Application.Common.ApplicationServices.Email;


/// <summary>
/// Represents an email attachment contract.
/// Uses byte[] for easy serialization (outbox pattern compatibility).
/// </summary>
public interface IAttachment
{
    /// <summary>
    /// Name of the attachment file (e.g., "report.pdf").
    /// </summary>
    string FileName { get; }

    /// <summary>
    /// Binary content of the attachment.
    /// </summary>
    byte[] Content { get; }
}

/// <summary>
/// Represents a single email request contract.
/// One instance = one email to one recipient.
/// </summary>
public interface IMailRequest
{
    /// <summary>
    /// Recipient email address.
    /// </summary>
    string To { get; }

    /// <summary>
    /// Email subject line.
    /// </summary>
    string Subject { get; }

    /// <summary>
    /// Email body content (HTML supported).
    /// </summary>
    string? Body { get; }

    /// <summary>
    /// Sender email address. Falls back to default if not specified.
    /// </summary>
    string? From { get; }

    /// <summary>
    /// Sender display name.
    /// </summary>
    string? DisplayName { get; }

    /// <summary>
    /// Reply-to email address.
    /// </summary>
    string? ReplyTo { get; }

    /// <summary>
    /// Reply-to display name.
    /// </summary>
    string? ReplyToName { get; }

    /// <summary>
    /// Blind carbon copy recipient.
    /// </summary>
    string? Bcc { get; }

    /// <summary>
    /// Carbon copy recipient.
    /// </summary>
    string? Cc { get; }

    /// <summary>
    /// File attachments.
    /// </summary>
    IReadOnlyList<IAttachment> Attachments { get; }

    /// <summary>
    /// Custom email headers.
    /// </summary>
    IReadOnlyDictionary<string, string> Headers { get; }
}
