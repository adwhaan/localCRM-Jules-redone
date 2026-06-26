using LocalCRM.Application.Common.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace LocalCRM.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly IPasswordHasher<string> _passwordHasher;

    public IdentityService()
    {
        _passwordHasher = new PasswordHasher<string>();
    }

    public string HashPassword(string password)
    {
        return _passwordHasher.HashPassword("user", password);
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        var result = _passwordHasher.VerifyHashedPassword("user", hashedPassword, password);
        return result == PasswordVerificationResult.Success;
    }
}
