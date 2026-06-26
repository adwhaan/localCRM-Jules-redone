using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using LocalCRM.Application.Common.Interfaces;
using LocalCRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace LocalCRM.Application.Auth;

public interface IAuthService
{
    Task<AuthResponse> LoginAsync(LoginCommand command, string? ipAddress, string? userAgent);
    Task<AuthResponse> RefreshTokenAsync(string refreshToken, string? ipAddress, string? userAgent);
    Task RevokeTokenAsync(string refreshToken, string? ipAddress);
    Task ChangePasswordAsync(string username, ChangePasswordCommand command);
}

public class AuthService : IAuthService
{
    private readonly ILocalCRMContext _context;
    private readonly IIdentityService _identityService;
    private readonly IConfiguration _configuration;

    public AuthService(ILocalCRMContext context, IIdentityService identityService, IConfiguration configuration)
    {
        _context = context;
        _identityService = identityService;
        _configuration = configuration;
    }

    public async Task<AuthResponse> LoginAsync(LoginCommand command, string? ipAddress, string? userAgent)
    {
        var user = await _context.Users
            .Include(u => u.Role)
                .ThenInclude(r => r.Permissions)
            .FirstOrDefaultAsync(u => u.Username == command.Username);

        if (user == null || !user.IsActive || !_identityService.VerifyPassword(command.Password, user.PasswordHash))
        {
            // Audit failed login
            _context.AuditLogs.Add(new AuditLog
            {
                EntityName = "users",
                ActionType = "AUTH_FAILURE",
                PerformedBy = command.Username,
                Notes = $"Failed login attempt for user: {command.Username}"
            });
            await _context.SaveChangesAsync();
            throw new Exception("Invalid credentials");
        }

        if (user.MustChangePassword)
        {
            return new AuthResponse { PasswordChangeRequired = true, Username = user.Username };
        }

        var accessToken = GenerateAccessToken(user);
        var refreshToken = await GenerateRefreshToken(user, ipAddress, userAgent);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.TokenHash, // For simplicity using hash as token in this demo, real world uses secure random string
            Username = user.Username,
            Permissions = user.Role.Permissions.Select(p => p.PermissionName).ToList()
        };
    }

    public async Task<AuthResponse> RefreshTokenAsync(string refreshToken, string? ipAddress, string? userAgent)
    {
        var tokenEntity = await _context.RefreshTokens
            .Include(t => t.User)
                .ThenInclude(u => u.Role)
                    .ThenInclude(r => r.Permissions)
            .FirstOrDefaultAsync(t => t.TokenHash == refreshToken);

        if (tokenEntity == null || tokenEntity.RevokedAt != null || tokenEntity.ExpiresAt < DateTime.UtcNow)
        {
            throw new Exception("Invalid or expired refresh token");
        }

        // Token rotation
        var user = tokenEntity.User;
        var newAccessToken = GenerateAccessToken(user);
        var newRefreshToken = await GenerateRefreshToken(user, ipAddress, userAgent);

        tokenEntity.RevokedAt = DateTime.UtcNow;
        tokenEntity.ReplacedByTokenId = newRefreshToken.RefreshTokenId;
        await _context.SaveChangesAsync();

        return new AuthResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken.TokenHash,
            Username = user.Username,
            Permissions = user.Role.Permissions.Select(p => p.PermissionName).ToList()
        };
    }

    public async Task RevokeTokenAsync(string refreshToken, string? ipAddress)
    {
        var tokenEntity = await _context.RefreshTokens.FirstOrDefaultAsync(t => t.TokenHash == refreshToken);
        if (tokenEntity != null)
        {
            tokenEntity.RevokedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task ChangePasswordAsync(string username, ChangePasswordCommand command)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null || !_identityService.VerifyPassword(command.OldPassword, user.PasswordHash))
        {
            throw new Exception("Invalid credentials");
        }

        user.PasswordHash = _identityService.HashPassword(command.NewPassword);
        user.MustChangePassword = false;
        user.UpdatedAt = DateTime.UtcNow;
        user.UpdatedBy = username;

        _context.AuditLogs.Add(new AuditLog
        {
            EntityName = "users",
            EntityId = user.UserId,
            ActionType = "UPDATE",
            PerformedBy = username,
            Notes = $"Changed password for user: {username}"
        });

        await _context.SaveChangesAsync();
    }

    private string GenerateAccessToken(User user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secret = jwtSettings.GetValue<string>("Secret");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role.RoleName),
            new Claim("uid", user.UserId.ToString())
        };

        foreach (var permission in user.Role.Permissions)
        {
            claims.Add(new Claim("permission", permission.PermissionName));
        }

        var token = new JwtSecurityToken(
            issuer: jwtSettings.GetValue<string>("Issuer"),
            audience: jwtSettings.GetValue<string>("Audience"),
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(jwtSettings.GetValue<int>("ExpiryMinutes")),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task<RefreshToken> GenerateRefreshToken(User user, string? ipAddress, string? userAgent)
    {
        var token = new RefreshToken
        {
            UserId = user.UserId,
            TokenHash = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            IssuedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedByIp = ipAddress,
            UserAgent = userAgent,
            SessionId = Guid.NewGuid().ToString()
        };

        _context.RefreshTokens.Add(token);
        await _context.SaveChangesAsync();
        return token;
    }
}
