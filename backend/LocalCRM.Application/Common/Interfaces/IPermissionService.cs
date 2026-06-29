using LocalCRM.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LocalCRM.Application.Common.Interfaces;

public interface IPermissionService
{
    Task<bool> HasPermissionAsync(string username, string permissionName);
    Task<List<string>> GetUserPermissionsAsync(string username);
}
