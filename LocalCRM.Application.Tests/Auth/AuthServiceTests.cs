using LocalCRM.Application.Auth;
using LocalCRM.Application.Common.Interfaces;
using LocalCRM.Domain.Entities;
using LocalCRM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace LocalCRM.Application.Tests.Auth;

public class AuthServiceTests
{
    private readonly Mock<IIdentityService> _identityServiceMock;
    private readonly IConfiguration _configuration;

    public AuthServiceTests()
    {
        _identityServiceMock = new Mock<IIdentityService>();

        var inMemoryConfig = new Dictionary<string, string> {
            {"JwtSettings:Secret", "SuperSecretKeyForTestingOnly1234567890"},
            {"JwtSettings:Issuer", "TestIssuer"},
            {"JwtSettings:Audience", "TestAudience"},
            {"JwtSettings:ExpiryMinutes", "60"}
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemoryConfig!)
            .Build();
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsAuthResponse()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<LocalCRMContext>()
            .UseInMemoryDatabase(databaseName: "LoginValid")
            .Options;

        using var context = new LocalCRMContext(options);

        var role = new Role { RoleName = "User" };
        context.Roles.Add(role);
        await context.SaveChangesAsync();

        var user = new User
        {
            Username = "testuser",
            PasswordHash = "hashed_password",
            RoleId = role.RoleId,
            IsActive = true
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        _identityServiceMock.Setup(x => x.VerifyPassword("password", "hashed_password")).Returns(true);

        var service = new AuthService(context, _identityServiceMock.Object, _configuration);

        // Act
        var result = await service.LoginAsync(new LoginCommand { Username = "testuser", Password = "password" }, "127.0.0.1", "TestAgent");

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.AccessToken);
        Assert.NotNull(result.RefreshToken);
        Assert.Equal("testuser", result.Username);
        Assert.False(result.PasswordChangeRequired);
    }
}
