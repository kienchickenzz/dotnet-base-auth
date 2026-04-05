/**
 * ExternalLoginCallbackResponse contains callback result.
 *
 * <p>Either contains token (existing user) or registration info (new user).</p>
 */
namespace AuthService.Application.Features.Identities.Authentication.Commands.ExternalLogin;

using AuthService.Application.Common.Abstractions.Identity.Models;
using AuthService.Application.Features.Identities.Authentication.Models.Responses;


/// <summary>
/// Response from external login callback.
/// </summary>
public sealed record ExternalLoginCallbackResponse
{
    /// <summary>
    /// True if user already exists and is logged in.
    /// </summary>
    public bool IsExistingUser { get; init; }

    /// <summary>
    /// JWT tokens (populated if existing user).
    /// </summary>
    public TokenResponse? Token { get; init; }

    /// <summary>
    /// External login info (populated if new user needs registration).
    /// </summary>
    public ExternalLoginInfoDto? ExternalLoginInfo { get; init; }
}
