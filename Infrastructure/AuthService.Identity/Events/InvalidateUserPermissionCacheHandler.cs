namespace AuthService.Identity.Events;

using Microsoft.AspNetCore.Identity;

using AuthService.Application.Common.Messaging;
using AuthService.Application.Common.Abstractions.Identity;
using AuthService.Identity.Entities;


internal class InvalidateUserPermissionCacheHandler :
    IDomainEventHandler<ApplicationUserUpdatedEvent>,
    IDomainEventHandler<ApplicationRoleUpdatedEvent>
{
    private readonly IIdentityPermissionService _identityPermissionService;
    private readonly UserManager<ApplicationUser> _userManager;

    public InvalidateUserPermissionCacheHandler(IIdentityPermissionService userService, UserManager<ApplicationUser> userManager) =>
        (_identityPermissionService, _userManager) = (userService, userManager);

    public async Task Handle(ApplicationUserUpdatedEvent notification, CancellationToken cancellationToken)
    {
        if (notification.IsRolesUpdated)
        {
            await _identityPermissionService.InvalidateCacheAsync(notification.UserId, cancellationToken);
        }
    }

    public async Task Handle(ApplicationRoleUpdatedEvent notification, CancellationToken cancellationToken)
    {
        if (notification.IsPermissionsUpdated)
        {
            foreach (var user in await _userManager.GetUsersInRoleAsync(notification.RoleName))
            {
                await _identityPermissionService.InvalidateCacheAsync(user.Id, cancellationToken);
            }
        }
    }
}
