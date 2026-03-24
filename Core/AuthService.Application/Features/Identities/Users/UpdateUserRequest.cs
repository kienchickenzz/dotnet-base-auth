namespace AuthService.Application.Features.Identities.Users;

using AuthService.Application.Common.ApplicationServices.FileStorage;


public record UpdateUserRequest
{
    public Guid Id { get; set; } = default!;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public FileUploadRequest? Image { get; set; }
    public bool IsDeleteCurrentImage { get; set; } = false;
}