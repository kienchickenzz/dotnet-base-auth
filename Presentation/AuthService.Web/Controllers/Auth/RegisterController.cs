/**
 * RegisterController handles user self-registration.
 *
 * <p>Only creates regular users. Admin accounts are created via Admin Dashboard.</p>
 */
namespace AuthService.Web.Controllers.Auth;

using MediatR;
using Microsoft.AspNetCore.Mvc;

using AuthService.Application.Common.Abstractions.Identity;
using AuthService.Application.Features.Identities.Authentication.Commands.Login;
using AuthService.Application.Features.Identities.Users.Commands.CreateUser;
using AuthService.Identity.Middlewares;
using AuthService.Web.Models.Auth;


/// <summary>
/// Shared controller for user registration.
/// </summary>
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
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home", new { area = "" });

        var model = new RegisterViewModel
        {
            ReturnUrl = returnUrl ?? Url.Content("~/"),
            ExternalLogins = await _externalAuthService.GetExternalAuthSchemesAsync()
        };

        return View("~/Views/Auth/Register.cshtml", model);
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
            return View("~/Views/Auth/Register.cshtml", model);

        var origin = $"{Request.Scheme}://{Request.Host.Value}";

        var createCommand = new CreateUserCommand(
            model.FirstName,
            model.LastName,
            model.Email,
            model.Email,
            model.Password,
            model.ConfirmPassword,
            model.PhoneNumber,
            origin);

        var createResult = await _mediator.Send(createCommand);

        if (createResult.IsFailure)
        {
            ModelState.AddModelError(string.Empty, createResult.Error.Name);
            return View("~/Views/Auth/Register.cshtml", model);
        }

        _logger.LogInformation("User {Email} registered successfully.", model.Email);

        // Auto-login after registration
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var loginCommand = new LoginCommand(model.Email, model.Password, ipAddress);
        var loginResult = await _mediator.Send(loginCommand);

        if (loginResult.IsSuccess)
        {
            _SetTokenCookies(loginResult.Value.Token, loginResult.Value.RefreshToken);
            // New user = Customer, redirect to Customer Profile
            return RedirectToAction("Index", "Profile", new { area = "Customer" });
        }

        TempData["Success"] = "Registration successful! Please login.";
        return RedirectToAction("Login", "Login");
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
            SameSite = SameSiteMode.Lax,
            Path = "/"
        };

        Response.Cookies.Append(JwtCookieMiddleware.AccessTokenCookieName, accessToken, cookieOptions);
        Response.Cookies.Append(JwtCookieMiddleware.RefreshTokenCookieName, refreshToken, cookieOptions);
    }
}
