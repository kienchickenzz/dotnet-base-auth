/**
 * UserRoleDto represents role assignment for a user.
 *
 * <p>Used for role queries and assignments.</p>
 */
namespace AuthService.Application.Common.Abstractions.Identity.Models;


/// <summary>
/// User role assignment data.
/// </summary>
public sealed record UserRoleDto
{
    /// <summary>Role's unique identifier.</summary>
    public Guid RoleId { get; init; }

    /// <summary>Role name.</summary>
    public string RoleName { get; init; } = default!;

    /// <summary>Role description.</summary>
    public string? Description { get; init; }

    /// <summary>Whether role is enabled for the user.</summary>
    public bool IsEnabled { get; init; }
}
