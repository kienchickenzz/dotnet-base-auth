/**
 * UpdateRoleCommandValidator validates role update data.
 *
 * <p>Checks role name uniqueness (excluding current role).</p>
 */
namespace AuthService.Application.Features.Identities.Roles.Commands.UpdateRole;

using FluentValidation;

using AuthService.Application.Common.Abstractions.Identity;


/// <summary>
/// Validator for UpdateRoleCommand.
/// </summary>
public sealed class UpdateRoleCommandValidator : AbstractValidator<UpdateRoleCommand>
{
    public UpdateRoleCommandValidator(IIdentityRoleService roleService)
    {
        RuleFor(r => r.Id).Cascade(CascadeMode.Stop)
            .NotEmpty();

        RuleFor(r => r.Name).Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MustAsync(async (cmd, name, _) => !await roleService.ExistsAsync(name, cmd.Id))
                .WithMessage((_, name) => $"Role {name} already exists.");
    }
}
