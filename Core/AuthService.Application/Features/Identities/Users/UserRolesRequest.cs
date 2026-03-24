namespace AuthService.Application.Features.Identities.Users;


public class UserRolesRequest
{
    public List<UserRoleResponse> UserRoles { get; set; } = new();
}