namespace LocalCRM.Application.Common.Interfaces;

public interface IIdentityService
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hashedPassword);
}
