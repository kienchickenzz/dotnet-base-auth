/**
 * SignInService implements cookie-based sign-in operations.
 *
 * <p>Wraps ASP.NET Identity SignInManager for session management.</p>
 */
namespace AuthService.Identity.Services;

using AuthService.Application.Features.Identities.Authentication.Services;
using AuthService.Identity.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

/// <summary>
/// Cookie-based sign-in service wrapping SignInManager.
/// </summary>
public sealed class SignInService : ISignInService
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    /// <summary>
    /// Initializes SignInService with Identity managers.
    /// </summary>
    public SignInService(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    /// <inheritdoc/>
    public async Task SignInAsync(Guid userId, bool rememberMe)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user is not null)
        {
            await _signInManager.SignInAsync(user, isPersistent: rememberMe);
        }
    }

    /// <inheritdoc/>
    public async Task SignOutAsync()
    {
        await _signInManager.SignOutAsync();
    }

    /// <inheritdoc/>
    public async Task SignOutExternalSchemeAsync()
    {
        await _signInManager.Context.SignOutAsync(IdentityConstants.ExternalScheme);
    }

    /// <inheritdoc/>
    public async Task<IList<AuthenticationScheme>> GetExternalAuthenticationSchemesAsync()
    {
        return (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
    }
}
