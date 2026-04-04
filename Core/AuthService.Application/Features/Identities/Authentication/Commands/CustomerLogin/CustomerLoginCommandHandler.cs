/**
 * CustomerLoginCommandHandler processes web-based customer login.
 *
 * <p>Reuses IAuthenticationService for validation, ISignInService for cookie.</p>
 */
namespace AuthService.Application.Features.Identities.Authentication.Commands.CustomerLogin;

using AuthService.Application.Common.Messaging;
using AuthService.Application.Features.Identities.Authentication.Services;
using AuthService.Domain.Common;

/// <summary>
/// Handler for CustomerLoginCommand - validates then creates cookie.
/// </summary>
public sealed class CustomerLoginCommandHandler
    : ICommandHandler<CustomerLoginCommand, CustomerLoginResult>
{
    private readonly IAuthenticationService _authService;
    private readonly ISignInService _signInService;

    /// <summary>
    /// Initializes handler with authentication and sign-in services.
    /// </summary>
    public CustomerLoginCommandHandler(
        IAuthenticationService authService,
        ISignInService signInService)
    {
        _authService = authService;
        _signInService = signInService;
    }

    /// <summary>
    /// Handles customer login - validates credentials then creates cookie.
    /// </summary>
    public async Task<Result<CustomerLoginResult>> Handle(
        CustomerLoginCommand request,
        CancellationToken cancellationToken)
    {
        // Step 1: Reuse - Validate credentials via IAuthenticationService
        var validateResult = await _authService.ValidateCredentialsAsync(
            request.Email,
            request.Password,
            cancellationToken);

        // Handle validation failures
        if (validateResult.IsFailure)
        {
            var error = validateResult.Error;

            // Map known error codes to login result
            return error.Code switch
            {
                "User.LockedOut" => new CustomerLoginResult(false, false, true),
                "User.RequiresTwoFactor" => new CustomerLoginResult(false, true, false),
                _ => Result.Failure<CustomerLoginResult>(error)
            };
        }

        // Step 2: Create authentication cookie via ISignInService
        var user = validateResult.Value;

        await _signInService.SignInAsync(user.Id, request.RememberMe);

        return new CustomerLoginResult(true, false, false);
    }
}
