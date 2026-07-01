using LocalCRM.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LocalCRM.Infrastructure.Services;

public class PermissionService : IPermissionService
{
    private readonly ILocalCRMContext _context;

    public PermissionService(ILocalCRMContext context)
    {
        _context = context;
    }

    public async Task<bool> HasPermissionAsync(string username, string permissionName)
    {
        var user = await _context.Users
            .Include(u => u.Role)
                .ThenInclude(r => r.Permissions)
            .FirstOrDefaultAsync(u => u.Username == username);

        if (user == null) return false;

        return user.Role.Permissions.Any(p => p.PermissionName == permissionName);
    }

    public async Task<List<string>> GetUserPermissionsAsync(string username)
    {
        var user = await _context.Users
            .Include(u => u.Role)
                .ThenInclude(r => r.Permissions)
            .FirstOrDefaultAsync(u => u.Username == username);

        if (user == null) return new List<string>();

        return user.Role.Permissions.Select(p => p.PermissionName).ToList();
    }
}
