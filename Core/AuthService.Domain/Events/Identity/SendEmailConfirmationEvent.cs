/**
 * SendEmailConfirmationEvent is raised when email confirmation needs to be sent.
 *
 * <p>Triggers email confirmation workflow via outbox pattern.</p>
 */
namespace AuthService.Domain.Events.Identity;

using AuthService.Domain.Common;


/// <summary>
/// Event raised when an email confirmation needs to be sent.
/// </summary>
/// <param name="UserId">The user's unique identifier.</param>
/// <param name="Email">The user's email address.</param>
/// <param name="FirstName">The user's first name.</param>
/// <param name="ConfirmationToken">The Base64Url-encoded confirmation token.</param>
/// <param name="Origin">The origin URL for building confirmation link.</param>
public sealed record SendEmailConfirmationEvent(
    Guid UserId,
    string Email,
    string FirstName,
    string ConfirmationToken,
    string Origin) : IDomainEvent;
