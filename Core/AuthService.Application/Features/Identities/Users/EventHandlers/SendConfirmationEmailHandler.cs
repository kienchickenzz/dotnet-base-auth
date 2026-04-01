/**
 * SendConfirmationEmailHandler processes SendEmailConfirmationEvent.
 *
 * <p>Sends email confirmation when user registers.
 * Invoked by outbox processor after event is persisted.</p>
 */
namespace AuthService.Application.Features.Identities.Users.EventHandlers;

using Microsoft.AspNetCore.WebUtilities;

using AuthService.Application.Common.ApplicationServices.Email;
using AuthService.Application.Common.Messaging;
using AuthService.Domain.Events.Identity;


/// <summary>
/// Handles SendEmailConfirmationEvent by sending confirmation email.
/// </summary>
internal sealed class SendConfirmationEmailHandler : IDomainEventHandler<SendEmailConfirmationEvent>
{
    private readonly IMailService _mailService;
    private readonly IMailRequestFactory _mailRequestFactory;
    private readonly IEmailTemplateFactory _emailTemplateFactory;

    public SendConfirmationEmailHandler(
        IMailService mailService,
        IMailRequestFactory mailRequestFactory,
        IEmailTemplateFactory emailTemplateFactory)
    {
        _mailService = mailService;
        _mailRequestFactory = mailRequestFactory;
        _emailTemplateFactory = emailTemplateFactory;
    }

    /// <inheritdoc />
    public async Task Handle(SendEmailConfirmationEvent notification, CancellationToken cancellationToken)
    {
        string confirmationUrl = _BuildConfirmationUrl(
            notification.Origin,
            notification.UserId,
            notification.ConfirmationToken);

        var model = new EmailConfirmationModel(
            notification.FirstName,
            notification.Email,
            confirmationUrl);

        string body = _emailTemplateFactory.GenerateEmailTemplate("email-confirmation", model);

        var request = _mailRequestFactory.Create(
            to: notification.Email,
            subject: "Confirm Your Email",
            body: body);

        await _mailService.SendAsync(request, cancellationToken);
    }

    /// <summary>
    /// Builds the confirmation URL with query parameters.
    /// </summary>
    private static string _BuildConfirmationUrl(string origin, Guid userId, string token)
    {
        const string route = "api/users/confirm-email";
        var endpointUri = new Uri(string.Concat($"{origin.TrimEnd('/')}/", route));

        var url = QueryHelpers.AddQueryString(endpointUri.ToString(), "userId", userId.ToString());
        url = QueryHelpers.AddQueryString(url, "code", token);

        return url;
    }
}


/// <summary>
/// Model for email confirmation template.
/// </summary>
public sealed record EmailConfirmationModel(
    string UserName,
    string Email,
    string Url);
