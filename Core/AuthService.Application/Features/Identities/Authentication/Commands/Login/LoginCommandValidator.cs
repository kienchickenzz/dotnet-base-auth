/**
 * LoginCommandValidator validates login request data.
 *
 * <p>Ensures email format and password presence before processing.</p>
 */
namespace AuthService.Application.Features.Identities.Authentication.Commands.Login;

using FluentValidation;


/// <summary>
/// Validator for LoginCommand.
/// </summary>
public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
                .WithMessage("Email is required.")
            .EmailAddress()
                .WithMessage("Invalid email format.");

        RuleFor(x => x.Password)
            .NotEmpty()
                .WithMessage("Password is required.");
    }
}
