/**
 * LoginController handles customer authentication login flow.
 *
 * <p>Thin controller - delegates to MediatR command handler.</p>
 */

using AuthService.Application.Features.Identities.Authentication.Commands.CustomerLogin;
using AuthService.Application.Features.Identities.Authentication.Services;
using AuthService.Web.Areas.Customer.Features.Auth.Login.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Web.Areas.Customer.Features.Auth.Login.Controllers;

/// <summary>
/// Controller for customer login functionality.
/// </summary>
[Area("Customer")]
public class LoginController : Controller
{
    private readonly IMediator _mediator;
    private readonly ISignInService _signInService;
    private readonly ILogger<LoginController> _logger;

    /// <summary>
    /// Initializes LoginController with required dependencies.
    /// </summary>
    public LoginController(
        IMediator mediator,
        ISignInService signInService,
        ILogger<LoginController> logger)
    {
        _mediator = mediator;
        _signInService = signInService;
        _logger = logger;
    }

    /// <summary>
    /// Displays the login form.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Login(string? returnUrl = null)
    {
        // Clear external cookie
        await _signInService.SignOutExternalSchemeAsync();

        var model = new LoginViewModel
        {
            ReturnUrl = returnUrl ?? Url.Content("~/"),
            ExternalLogins = await _signInService.GetExternalAuthenticationSchemesAsync()
        };

        return View(model);
    }

    /// <summary>
    /// Processes the login form submission.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        model.ReturnUrl ??= Url.Content("~/");
        model.ExternalLogins = await _signInService.GetExternalAuthenticationSchemesAsync();

        if (!ModelState.IsValid)
            return View(model);

        // Thin controller: delegate to MediatR
        var command = new CustomerLoginCommand(
            model.Email,
            model.Password,
            model.RememberMe);

        var result = await _mediator.Send(command);

        // Handle failure
        if (result.IsFailure)
        {
            ModelState.AddModelError(string.Empty, result.Error.Name);
            return View(model);
        }

        var loginResult = result.Value;

        // Handle success cases
        if (loginResult.Succeeded)
        {
            _logger.LogInformation("User {Email} logged in.", model.Email);
            return LocalRedirect(model.ReturnUrl);
        }

        if (loginResult.RequiresTwoFactor)
        {
            return RedirectToAction("LoginWith2fa", new
            {
                ReturnUrl = model.ReturnUrl,
                RememberMe = model.RememberMe
            });
        }

        if (loginResult.IsLockedOut)
        {
            _logger.LogWarning("User {Email} account locked out.", model.Email);
            return RedirectToAction("Lockout");
        }

        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        return View(model);
    }
}
