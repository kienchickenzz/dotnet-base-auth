/**
 * ResetPasswordCommandValidator validates password reset data.
 *
 * <p>Checks email, token, and password requirements.</p>
 */
namespace AuthService.Application.Features.Identities.Users.Commands.Password;

using FluentValidation;


/// <summary>
/// Validator for ResetPasswordCommand.
/// </summary>
public sealed class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(p => p.Email).Cascade(CascadeMode.Stop)
            .NotEmpty()
            .EmailAddress()
                .WithMessage("Invalid Email Address.");

        RuleFor(p => p.Token).Cascade(CascadeMode.Stop)
            .NotEmpty();

        RuleFor(p => p.NewPassword).Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MinimumLength(6);

        RuleFor(p => p.ConfirmNewPassword).Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Equal(p => p.NewPassword)
                .WithMessage("Passwords do not match.");
    }
}
