/**
 * UserDto represents user data for Application layer.
 *
 * <p>Framework-agnostic user representation.</p>
 */
namespace AuthService.Application.Common.Abstractions.Identity.Models;


/// <summary>
/// User data transfer object.
/// </summary>
public sealed record UserDto
{
    /// <summary>User's unique identifier.</summary>
    public Guid Id { get; init; }

    /// <summary>User's username.</summary>
    public string UserName { get; init; } = default!;

    /// <summary>User's email address.</summary>
    public string Email { get; init; } = default!;

    /// <summary>User's first name.</summary>
    public string? FirstName { get; init; }

    /// <summary>User's last name.</summary>
    public string? LastName { get; init; }

    /// <summary>User's phone number.</summary>
    public string? PhoneNumber { get; init; }

    /// <summary>User's profile image URL.</summary>
    public string? ImageUrl { get; init; }

    /// <summary>Whether user account is active.</summary>
    public bool IsActive { get; init; }

    /// <summary>Whether email is confirmed.</summary>
    public bool EmailConfirmed { get; init; }

    /// <summary>Whether phone is confirmed.</summary>
    public bool PhoneNumberConfirmed { get; init; }
}
