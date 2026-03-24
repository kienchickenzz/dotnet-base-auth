namespace AuthService.Application.Features.Identities.Authentication;


public interface ITokenService
{
    Task<LoginResponse> GetTokenAsync(LoginRequest request, string ipAddress, CancellationToken cancellationToken);

    Task<LoginResponse> RefreshTokenAsync(RefreshTokenRequest request, string ipAddress);
}