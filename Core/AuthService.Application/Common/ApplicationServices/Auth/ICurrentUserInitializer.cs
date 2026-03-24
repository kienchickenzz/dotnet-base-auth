namespace AuthService.Application.Common.ApplicationServices.Auth;

using System.Security.Claims;


public interface ICurrentUserInitializer
{
    void SetCurrentUser(ClaimsPrincipal user);

    void SetCurrentUserId(string userId);
}
