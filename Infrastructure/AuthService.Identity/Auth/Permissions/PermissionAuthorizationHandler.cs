namespace AuthService.Identity.Auth.Permissions;

using Microsoft.AspNetCore.Authorization;

using AuthService.Application.Common.Extensions.Identity;
using AuthService.Application.Common.Abstractions.Identity;


internal class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IIdentityPermissionService _identityPermissionService;

    public PermissionAuthorizationHandler(IIdentityPermissionService identityPermissionService) =>
        _identityPermissionService = identityPermissionService;

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        if (context.User?.GetUserId() is { } userId &&
            await _identityPermissionService.HasPermissionAsync(new Guid(userId), requirement.Permission))
        {
            context.Succeed(requirement);
        }
    }
}