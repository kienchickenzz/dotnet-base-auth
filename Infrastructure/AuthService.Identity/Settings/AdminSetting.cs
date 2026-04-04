namespace AuthService.Identity.Settings;


public class AdminSettings
{   
    public const string SectionName = "SecuritySettings:AdminSettings";

    public required string FirstName { get; set; }
    public required  string LastName { get; set; }
    public required  string Email { get; set; }
    public required  string UserName { get; set; }
    public required  string Password { get; set; }
}
