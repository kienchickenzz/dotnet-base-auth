/**
 * UpdateUserCommandValidator validates user update data.
 *
 * <p>Checks email and phone uniqueness (excluding current user).</p>
 */
namespace AuthService.Application.Features.Identities.Users.Commands.UpdateUser;

using FluentValidation;

using AuthService.Application.Common.Abstractions.Identity;


/// <summary>
/// Validator for UpdateUserCommand.
/// </summary>
public sealed class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator(IIdentityUserService userService)
    {
        RuleFor(u => u.Id).Cascade(CascadeMode.Stop)
            .NotEmpty();

        RuleFor(u => u.Email).Cascade(CascadeMode.Stop)
            .NotEmpty()
            .EmailAddress()
                .WithMessage("Invalid Email Address.")
            .MustAsync(async (cmd, email, _) => !await userService.ExistsWithEmailAsync(email, cmd.Id))
                .WithMessage((_, email) => $"Email {email} is already registered.");

        RuleFor(u => u.PhoneNumber).Cascade(CascadeMode.Stop)
            .MustAsync(async (cmd, phone, _) => !await userService.ExistsWithPhoneNumberAsync(phone!, cmd.Id))
                .WithMessage((_, phone) => $"Phone number {phone} is already registered.")
                .Unless(u => string.IsNullOrWhiteSpace(u.PhoneNumber));

        RuleFor(p => p.FirstName).Cascade(CascadeMode.Stop)
            .NotEmpty();

        RuleFor(p => p.LastName).Cascade(CascadeMode.Stop)
            .NotEmpty();
    }
}
