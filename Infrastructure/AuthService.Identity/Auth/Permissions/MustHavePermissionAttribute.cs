namespace AuthService.Identity.Auth.Permissions;

using Microsoft.AspNetCore.Authorization;

using AuthService.Application.Features.Identities.Roles;


public class MustHavePermissionAttribute : AuthorizeAttribute
{
    public MustHavePermissionAttribute(string action, string resource) =>
        Policy = Permission.NameFor(action, resource);
}