/**
 * RoleDto represents role data for Application layer.
 *
 * <p>Framework-agnostic role representation.</p>
 */
namespace AuthService.Application.Common.Abstractions.Identity.Models;


/// <summary>
/// Role data transfer object.
/// </summary>
public sealed record RoleDto
{
    /// <summary>Role's unique identifier.</summary>
    public Guid Id { get; init; }

    /// <summary>Role name.</summary>
    public string Name { get; init; } = default!;

    /// <summary>Role description.</summary>
    public string? Description { get; init; }

    /// <summary>Role permissions.</summary>
    public List<string> Permissions { get; init; } = new();
}
