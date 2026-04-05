/**
 * ProfileController handles user profile display for Customer area.
 *
 * <p>Requires authentication. Shows current user's profile information.</p>
 */
namespace AuthService.Web.Areas.Customer.Features.Profile.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using AuthService.Application.Common.Abstractions.Identity;
using AuthService.Application.Common.ApplicationServices.Auth;
using AuthService.Web.Areas.Customer.Features.Profile.Models;


/// <summary>
/// Controller for user profile display.
/// </summary>
[Area("Customer")]
[Authorize]
public class ProfileController : Controller
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
        // Get current user ID from JWT claims
        var userId = _currentUser.GetUserId();
        if (userId == Guid.Empty)
            return RedirectToAction("Login", "Login", new { area = "Customer" });

        // Get user profile from database
        var userResult = await _userService.GetByIdAsync(userId);
        if (userResult.IsFailure)
            return NotFound();

        var user = userResult.Value;

        // Get user's roles (returns UserRoleDto with IsEnabled flag)
        var rolesResult = await _userService.GetRolesAsync(userId);
        var roles = rolesResult.IsSuccess
            ? rolesResult.Value.Where(r => r.IsEnabled).Select(r => r.RoleName).ToList()
            : new List<string>();

        // Get linked external providers
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
