/**
 * RegisterController handles customer registration using JWT.
 *
 * <p>Thin controller - delegates to MediatR, stores JWT in HttpOnly cookie.</p>
 */
namespace AuthService.Web.Areas.Customer.Features.Auth.Register.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;

using AuthService.Application.Common.Abstractions.Identity;
using AuthService.Application.Features.Identities.Authentication.Commands.Login;
using AuthService.Application.Features.Identities.Users.Commands.CreateUser;
using AuthService.Identity.Middlewares;
using AuthService.Web.Areas.Customer.Features.Auth.Register.Models;


/// <summary>
/// Controller for customer registration functionality.
/// </summary>
[Area("Customer")]
public class RegisterController : Controller
{
    private readonly IMediator _mediator;
    private readonly IExternalAuthService _externalAuthService;
    private readonly ILogger<RegisterController> _logger;

    /// <summary>
    /// Initializes RegisterController with required dependencies.
    /// </summary>
    public RegisterController(
        IMediator mediator,
        IExternalAuthService externalAuthService,
        ILogger<RegisterController> logger)
    {
        _mediator = mediator;
        _externalAuthService = externalAuthService;
        _logger = logger;
    }

    /// <summary>
    /// Displays the registration form.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Register(string? returnUrl = null)
    {
        // Redirect if already logged in
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home", new { area = "" });

        var model = new RegisterViewModel
        {
            ReturnUrl = returnUrl ?? Url.Content("~/"),
            ExternalLogins = await _externalAuthService.GetExternalAuthSchemesAsync()
        };

        return View(model);
    }

    /// <summary>
    /// Processes the registration form submission.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        model.ReturnUrl ??= Url.Content("~/");
        model.ExternalLogins = await _externalAuthService.GetExternalAuthSchemesAsync();

        if (!ModelState.IsValid)
            return View(model);

        // Get origin for confirmation email
        var origin = $"{Request.Scheme}://{Request.Host.Value}";

        // Create user via CQRS
        var createCommand = new CreateUserCommand(
            model.FirstName,
            model.LastName,
            model.Email,
            model.Email, // UserName = Email
            model.Password,
            model.ConfirmPassword,
            model.PhoneNumber,
            origin);

        var createResult = await _mediator.Send(createCommand);

        if (createResult.IsFailure)
        {
            ModelState.AddModelError(string.Empty, createResult.Error.Name);
            return View(model);
        }

        _logger.LogInformation("User {Email} registered successfully.", model.Email);

        // Auto-login after registration
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var loginCommand = new LoginCommand(model.Email, model.Password, ipAddress);
        var loginResult = await _mediator.Send(loginCommand);

        if (loginResult.IsSuccess)
        {
            _SetTokenCookies(loginResult.Value.Token, loginResult.Value.RefreshToken);
            return LocalRedirect(model.ReturnUrl);
        }

        // If auto-login fails, redirect to login page
        TempData["Success"] = "Registration successful! Please login.";
        return RedirectToAction("Login", "Login", new { area = "Customer", returnUrl = model.ReturnUrl });
    }

    /// <summary>
    /// Sets JWT tokens in HttpOnly cookies.
    /// </summary>
    private void _SetTokenCookies(string accessToken, string refreshToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict
        };

        Response.Cookies.Append(JwtCookieMiddleware.AccessTokenCookieName, accessToken, cookieOptions);
        Response.Cookies.Append(JwtCookieMiddleware.RefreshTokenCookieName, refreshToken, cookieOptions);
    }
}
