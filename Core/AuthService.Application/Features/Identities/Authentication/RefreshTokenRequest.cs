namespace AuthService.Application.Features.Identities.Authentication;


// TODO: Khi muốn refresh access token, chỉ cần refresh token thôi!! 
public record RefreshTokenRequest(string Token, string RefreshToken);