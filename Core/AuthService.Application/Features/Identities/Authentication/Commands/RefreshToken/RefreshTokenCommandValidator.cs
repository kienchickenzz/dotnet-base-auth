/**
 * RefreshTokenCommandValidator validates refresh token request data.
 *
 * <p>Ensures both tokens are present before processing.</p>
 */
namespace AuthService.Application.Features.Identities.Authentication.Commands.RefreshToken;

using FluentValidation;


/// <summary>
/// Validator for RefreshTokenCommand.
/// </summary>
public sealed class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty()
                .WithMessage("Access token is required.");

        RuleFor(x => x.RefreshToken)
            .NotEmpty()
                .WithMessage("Refresh token is required.");
    }
}
