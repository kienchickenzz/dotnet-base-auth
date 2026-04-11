/**
 * ProfileController handles user profile display for Admin area.
 *
 * <p>Inherits Admin role requirement from AdminBaseController.</p>
 */
namespace AuthService.Web.Areas.Admin.Features.Profile.Controllers;

using Microsoft.AspNetCore.Mvc;

using AuthService.Application.Common.Abstractions.Identity;
using AuthService.Application.Common.ApplicationServices.Auth;
using AuthService.Web.Areas.Admin.Features.Profile.Models;


/// <summary>
/// Controller for admin user profile display.
/// </summary>
public class ProfileController : AdminBaseController
{
    private readonly IIdentityUserService _userService;
    private readonly ICurrentUser _currentUser;

    /// <summary>
    /// Initializes ProfileController with required dependencies.
    /// </summary>
    public ProfileController(
        IIdentityUserService userService,
        ICurrentUser currentUser)
    {
        _userService = userService;
        _currentUser = currentUser;
    }

    /// <summary>
    /// Displays current user's profile.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = _currentUser.GetUserId();
        if (userId == Guid.Empty)
            return RedirectToAction("Login", "Login", new { area = "" });

        var userResult = await _userService.GetByIdAsync(userId);
        if (userResult.IsFailure)
            return NotFound();

        var user = userResult.Value;

        var rolesResult = await _userService.GetRolesAsync(userId);
        var roles = rolesResult.IsSuccess
            ? rolesResult.Value.Where(r => r.IsEnabled).Select(r => r.RoleName).ToList()
            : new List<string>();

        var externalLoginsResult = await _userService.GetExternalLoginsAsync(userId);
        var linkedProviders = externalLoginsResult.IsSuccess
            ? externalLoginsResult.Value.Select(l => l.ProviderDisplayName).ToList()
            : new List<string>();

        var model = new ProfileViewModel
        {
            Email = user.Email,
            FirstName = user.FirstName ?? string.Empty,
            LastName = user.LastName ?? string.Empty,
            PhoneNumber = user.PhoneNumber,
            ImageUrl = user.ImageUrl,
            EmailConfirmed = user.EmailConfirmed,
            PhoneNumberConfirmed = user.PhoneNumberConfirmed,
            Roles = roles,
            LinkedProviders = linkedProviders
        };

        return View(model);
    }
}
