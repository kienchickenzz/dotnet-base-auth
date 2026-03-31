/**
 * DeleteUserCommandValidator validates delete user requests.
 */
namespace AuthService.Application.Features.Identities.Users.Commands.DeleteUser;

using FluentValidation;


/// <summary>
/// Validator for DeleteUserCommand.
/// </summary>
public sealed class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required.");
    }
}
