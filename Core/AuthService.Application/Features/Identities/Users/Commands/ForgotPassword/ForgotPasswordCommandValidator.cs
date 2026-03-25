/**
 * ForgotPasswordCommandValidator validates forgot password request.
 *
 * <p>Checks email format and origin URL.</p>
 */
namespace AuthService.Application.Features.Identities.Users.Commands.Password;

using FluentValidation;


/// <summary>
/// Validator for ForgotPasswordCommand.
/// </summary>
public sealed class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator()
    {
        RuleFor(p => p.Email).Cascade(CascadeMode.Stop)
            .NotEmpty()
            .EmailAddress()
                .WithMessage("Invalid Email Address.");

        RuleFor(p => p.Origin).Cascade(CascadeMode.Stop)
            .NotEmpty();
    }
}
