/**
 * CreateRoleCommandValidator validates role creation data.
 *
 * <p>Checks role name uniqueness.</p>
 */
namespace AuthService.Application.Features.Identities.Roles.Commands.CreateRole;

using FluentValidation;

using AuthService.Application.Common.Abstractions.Identity;


/// <summary>
/// Validator for CreateRoleCommand.
/// </summary>
public sealed class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleCommandValidator(IIdentityRoleService roleService)
    {
        RuleFor(r => r.Name).Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MustAsync(async (name, _) => !await roleService.ExistsAsync(name))
                .WithMessage((_, name) => $"Role {name} already exists.");
    }
}
