namespace LocalCRM.Application.Auth;

public class LoginCommand
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class AuthResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public List<string> Permissions { get; set; } = new();
    public bool PasswordChangeRequired { get; set; }
}

public class RefreshTokenCommand
{
    public string RefreshToken { get; set; } = string.Empty;
}

public class ChangePasswordCommand
{
    public string OldPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}
