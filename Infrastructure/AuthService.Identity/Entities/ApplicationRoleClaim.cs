namespace AuthService.Identity.Entities;

using Microsoft.AspNetCore.Identity;


public class ApplicationRoleClaim : IdentityRoleClaim<Guid>
{
    public string? CreatedBy { get; init; }
    public DateTime CreatedOn { get; init; }
}