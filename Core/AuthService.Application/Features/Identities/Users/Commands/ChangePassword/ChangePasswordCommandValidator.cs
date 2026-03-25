/**
 * ChangePasswordCommandValidator validates password change data.
 *
 * <p>Checks password requirements and confirmation match.</p>
 */
namespace AuthService.Application.Features.Identities.Users.Commands.Password;

using FluentValidation;


/// <summary>
/// Validator for ChangePasswordCommand.
/// </summary>
public sealed class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(p => p.UserId).Cascade(CascadeMode.Stop)
            .NotEmpty();

        RuleFor(p => p.CurrentPassword).Cascade(CascadeMode.Stop)
            .NotEmpty();

        RuleFor(p => p.NewPassword).Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MinimumLength(6)
            .NotEqual(p => p.CurrentPassword)
                .WithMessage("New password must be different from current password.");

        RuleFor(p => p.ConfirmNewPassword).Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Equal(p => p.NewPassword)
                .WithMessage("Passwords do not match.");
    }
}
