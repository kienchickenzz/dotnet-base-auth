namespace AuthService.Identity.Auth.Permissions;

using Microsoft.AspNetCore.Authorization;

using AuthService.Application.Features.Identities;
using AuthService.Application.Features.Identities.Users;


internal class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IUserService _userService;

    public PermissionAuthorizationHandler(IUserService userService) =>
        _userService = userService;

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        if (context.User?.GetUserId() is { } userId &&
            await _userService.HasPermissionAsync(new Guid(userId), requirement.Permission))
        {
            context.Succeed(requirement);
        }
    }
}