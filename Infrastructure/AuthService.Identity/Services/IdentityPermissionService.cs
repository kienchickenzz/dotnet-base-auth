/**
 * IdentityPermissionService implements IIdentityPermissionService.
 *
 * <p>Provides permission queries with caching support.</p>
 */
namespace AuthService.Identity.Services;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using AuthService.Application.Common.Abstractions.Identity;
using AuthService.Application.Common.ApplicationServices.Caching;
using AuthService.Domain.Common;
using AuthService.Domain.Constants.Identity;
using AuthService.Identity.DatabaseContext;
using AuthService.Identity.Entities;


/// <summary>
/// Permission service with caching support.
/// </summary>
internal sealed class IdentityPermissionService : IIdentityPermissionService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ApplicationIdentityDbContext _db;
    private readonly ICacheService _cache;
    private readonly ICacheKeyService _cacheKeys;

    public IdentityPermissionService(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        ApplicationIdentityDbContext db,
        ICacheService cache,
        ICacheKeyService cacheKeys)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _db = db;
        _cache = cache;
        _cacheKeys = cacheKeys;
    }

    /// <inheritdoc />
    public async Task<Result<List<string>>> GetPermissionsAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user is null)
        {
            return Result.Failure<List<string>>(PermissionErrors.UserNotFound);
        }

        var userRoles = await _userManager.GetRolesAsync(user);
        var permissions = new List<string>();

        foreach (var role in await _roleManager.Roles
            .Where(r => userRoles.Contains(r.Name!))
            .ToListAsync(cancellationToken))
        {
            permissions.AddRange(await _db.RoleClaims
                .Where(rc => rc.RoleId == role.Id && rc.ClaimType == Claims.Permission)
                .Select(rc => rc.ClaimValue!)
                .ToListAsync(cancellationToken));
        }

        return permissions.Distinct().ToList();
    }

    /// <inheritdoc />
    public async Task<bool> HasPermissionAsync(
        Guid userId,
        string permission,
        CancellationToken cancellationToken = default)
    {
        var permissions = await _cache.GetOrSetAsync(
            _cacheKeys.GetCacheKey(Claims.Permission, userId),
            () => _GetPermissionsInternalAsync(userId, cancellationToken),
            cancellationToken: cancellationToken);

        return permissions?.Contains(permission) ?? false;
    }

    /// <inheritdoc />
    public Task InvalidateCacheAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return _cache.RemoveAsync(
            _cacheKeys.GetCacheKey(Claims.Permission, userId),
            cancellationToken);
    }

    /// <summary>
    /// Gets permissions from database (internal, for caching).
    /// </summary>
    private async Task<List<string>> _GetPermissionsInternalAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user is null)
        {
            return new List<string>();
        }

        var userRoles = await _userManager.GetRolesAsync(user);
        var permissions = new List<string>();

        foreach (var role in await _roleManager.Roles
            .Where(r => userRoles.Contains(r.Name!))
            .ToListAsync(cancellationToken))
        {
            permissions.AddRange(await _db.RoleClaims
                .Where(rc => rc.RoleId == role.Id && rc.ClaimType == Claims.Permission)
                .Select(rc => rc.ClaimValue!)
                .ToListAsync(cancellationToken));
        }

        return permissions.Distinct().ToList();
    }
}


/// <summary>
/// Error definitions for permission operations.
/// </summary>
internal static class PermissionErrors
{
    public static readonly Error UserNotFound = new("Permission.UserNotFound", "User Not Found.");
}
