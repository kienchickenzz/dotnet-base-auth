/**
 * ForgotPasswordCommand initiates password reset flow.
 *
 * <p>Processed by ForgotPasswordCommandHandler.</p>
 */
namespace AuthService.Application.Features.Identities.Users.Commands.Password;

using AuthService.Application.Common.Messaging;


/// <summary>
/// Command to initiate forgot password flow.
/// </summary>
/// <param name="Email">User's email address.</param>
/// <param name="Origin">Request origin URL for reset link.</param>
public sealed record ForgotPasswordCommand(
    string Email,
    string Origin) : ICommand<string>;
