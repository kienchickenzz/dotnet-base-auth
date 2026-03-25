/**
 * LoginCommand represents a user authentication request.
 *
 * <p>Processed by LoginCommandHandler to validate credentials and generate tokens.</p>
 */
namespace AuthService.Application.Features.Identities.Authentication.Commands.Login;

using AuthService.Application.Common.Messaging;
using AuthService.Application.Features.Identities.Authentication.Models.Responses;


/// <summary>
/// Command to authenticate a user and generate JWT tokens.
/// </summary>
/// <param name="Email">User's email address.</param>
/// <param name="Password">User's password.</param>
/// <param name="IpAddress">Client's IP address for token claims.</param>
public sealed record LoginCommand(
    string Email,
    string Password,
    string IpAddress) : ICommand<TokenResponse>;
