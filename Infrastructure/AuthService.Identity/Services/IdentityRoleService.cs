/**
 * IdentityRoleService implements IIdentityRoleService using ASP.NET Core Identity.
 *
 * <p>Provides role management operations.</p>
 */
namespace AuthService.Identity.Services;

using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using AuthService.Application.Common.Abstractions.Identity;
using AuthService.Application.Common.Abstractions.Identity.Models;
using AuthService.Application.Common.ApplicationServices.Auth;
using AuthService.Application.Features.Identities.Roles;
using AuthService.Domain.Common;
using AuthService.Domain.Constants.Identity;
using AuthService.Domain.Events.Identity;
using AuthService.Identity.DatabaseContext;
using AuthService.Identity.Entities;
using AuthService.Identity.Extensions;


/// <summary>
/// Role management service using ASP.NET Core Identity.
/// </summary>
internal sealed class IdentityRoleService : IIdentityRoleService
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationIdentityDbContext _db;
    private readonly ICurrentUser _currentUser;
    private readonly IPublisher _mediator;

    public IdentityRoleService(
        RoleManager<ApplicationRole> roleManager,
        UserManager<ApplicationUser> userManager,
        ApplicationIdentityDbContext db,
        ICurrentUser currentUser,
        IPublisher mediator)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _db = db;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    /// <inheritdoc />
    public async Task<Result<List<RoleDto>>> GetRolesAsync(CancellationToken cancellationToken = default)
    {
        var roles = await _roleManager.Roles.ToListAsync(cancellationToken);

        return roles.Select(r => new RoleDto
        {
            Id = r.Id,
            Name = r.Name!,
            Description = r.Description
        }).ToList();
    }

    /// <inheritdoc />
    public async Task<Result<RoleDto>> GetByIdAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        var role = await _db.Roles.SingleOrDefaultAsync(x => x.Id == roleId, cancellationToken);

        if (role is null)
        {
            return Result.Failure<RoleDto>(RoleErrors.NotFound);
        }

        return new RoleDto
        {
            Id = role.Id,
            Name = role.Name!,
            Description = role.Description
        };
    }

    /// <inheritdoc />
    public async Task<Result<RoleDto>> GetByIdWithPermissionsAsync(
        Guid roleId,
        CancellationToken cancellationToken = default)
    {
        var role = await _db.Roles.SingleOrDefaultAsync(x => x.Id == roleId, cancellationToken);

        if (role is null)
        {
            return Result.Failure<RoleDto>(RoleErrors.NotFound);
        }

        var permissions = await _db.RoleClaims
            .Where(c => c.RoleId == roleId && c.ClaimType == Claims.Permission)
            .Select(c => c.ClaimValue!)
            .ToListAsync(cancellationToken);

        return new RoleDto
        {
            Id = role.Id,
            Name = role.Name!,
            Description = role.Description,
            Permissions = permissions
        };
    }

    /// <inheritdoc />
    public Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return _roleManager.Roles.CountAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> ExistsAsync(string roleName, Guid? excludeId = null)
    {
        return await _roleManager.FindByNameAsync(roleName) is ApplicationRole existingRole
            && existingRole.Id != excludeId;
    }

    /// <inheritdoc />
    public async Task<Result<Guid>> CreateOrUpdateAsync(
        CreateOrUpdateRoleDto roleDto,
        CancellationToken cancellationToken = default)
    {
        if (roleDto.Id is null || roleDto.Id == Guid.Empty)
        {
            // Create a new role
            var role = new ApplicationRole(roleDto.Name, roleDto.Description);
            var result = await _roleManager.CreateAsync(role);

            if (!result.Succeeded)
            {
                var errors = result.GetErrors();
                return Result.Failure<Guid>(new Error(
                    "Role.CreateFailed",
                    string.Join(", ", errors.Select(e => e.Name))));
            }

            return role.Id;
        }
        else
        {
            // Update an existing role
            var role = await _roleManager.FindByIdAsync(roleDto.Id.ToString()!);

            if (role is null)
            {
                return Result.Failure<Guid>(RoleErrors.NotFound);
            }

            if (Roles.IsDefault(role.Name!))
            {
                return Result.Failure<Guid>(new Error(
                    "Role.CannotModifyDefault",
                    $"Not allowed to modify {role.Name} Role."));
            }

            role.Name = roleDto.Name;
            role.NormalizedName = roleDto.Name.ToUpperInvariant();
            role.Description = roleDto.Description;

            var result = await _roleManager.UpdateAsync(role);

            if (!result.Succeeded)
            {
                var errors = result.GetErrors();
                return Result.Failure<Guid>(new Error(
                    "Role.UpdateFailed",
                    string.Join(", ", errors.Select(e => e.Name))));
            }

            await _mediator.Publish(new RoleUpdatedEvent(role.Id), cancellationToken);

            return role.Id;
        }
    }

    /// <inheritdoc />
    public async Task<Result> DeleteAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        var role = await _roleManager.FindByIdAsync(roleId.ToString());

        if (role is null)
        {
            return Result.Failure(RoleErrors.NotFound);
        }

        if (Roles.IsDefault(role.Name!))
        {
            return Result.Failure(new Error(
                "Role.CannotDeleteDefault",
                $"Not allowed to delete {role.Name} Role."));
        }

        if ((await _userManager.GetUsersInRoleAsync(role.Name!)).Count > 0)
        {
            return Result.Failure(new Error(
                "Role.InUse",
                $"Not allowed to delete {role.Name} Role as it is being used."));
        }

        await _roleManager.DeleteAsync(role);

        return Result.Success();
    }

    /// <inheritdoc />
    public async Task<Result> UpdatePermissionsAsync(
        Guid roleId,
        List<string> permissions,
        CancellationToken cancellationToken = default)
    {
        var role = await _roleManager.FindByIdAsync(roleId.ToString());

        if (role is null)
        {
            return Result.Failure(RoleErrors.NotFound);
        }

        if (role.Name == Roles.Admin)
        {
            return Result.Failure(new Error(
                "Role.CannotModifyAdminPermissions",
                "Not allowed to modify Permissions for this Role."));
        }

        var currentClaims = await _roleManager.GetClaimsAsync(role);

        // Remove permissions that were previously selected
        foreach (var claim in currentClaims.Where(c => !permissions.Any(p => p == c.Value)))
        {
            var removeResult = await _roleManager.RemoveClaimAsync(role, claim);
            if (!removeResult.Succeeded)
            {
                var errors = removeResult.GetErrors();
                return Result.Failure(new Error(
                    "Role.UpdatePermissionsFailed",
                    string.Join(", ", errors.Select(e => e.Name))));
            }
        }

        // Add all permissions that were not previously selected
        foreach (string permission in permissions.Where(c => !currentClaims.Any(p => p.Value == c)))
        {
            if (!string.IsNullOrEmpty(permission))
            {
                _db.RoleClaims.Add(new ApplicationRoleClaim
                {
                    RoleId = role.Id,
                    ClaimType = Claims.Permission,
                    ClaimValue = permission,
                    CreatedBy = _currentUser.GetUserId(),
                    CreatedOn = DateTime.UtcNow
                });
                // SaveChanges is handled by TransactionPipelineBehavior
            }
        }

        await _mediator.Publish(new RoleUpdatedEvent(role.Id, true), cancellationToken);

        return Result.Success();
    }
}


/// <summary>
/// Error definitions for role operations.
/// </summary>
internal static class RoleErrors
{
    public static readonly Error NotFound = new("Role.NotFound", "Role Not Found.");
}
