using LocalCRM.Application.Common.Interfaces;
using LocalCRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LocalCRM.Infrastructure.Persistence;

public static class LocalCRMContextSeed
{
    public static async Task SeedDefaultDataAsync(LocalCRMContext context, IIdentityService identityService)
    {
        if (!context.Roles.Any())
        {
            var adminRole = new Role { RoleName = "Administrator" };
            var userRole = new Role { RoleName = "User" };

            context.Roles.AddRange(adminRole, userRole);
            await context.SaveChangesAsync();

            // Seed Permissions
            var permissions = new List<Permission>
            {
                new Permission { PermissionName = "read_all" },
                new Permission { PermissionName = "manage_users" },
                new Permission { PermissionName = "write_business" }
            };
            context.Permissions.AddRange(permissions);
            await context.SaveChangesAsync();

            // Assign all to admin
            foreach (var p in permissions) adminRole.Permissions.Add(p);

            // Assign read_all to user
            userRole.Permissions.Add(permissions.First(x => x.PermissionName == "read_all"));

            await context.SaveChangesAsync();

            // Seed Admin User
            if (!context.Users.Any())
            {
                context.Users.Add(new User
                {
                    Username = "admin",
                    PasswordHash = identityService.HashPassword("Admin123!"),
                    RoleId = adminRole.RoleId,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "system",
                    MustChangePassword = false
                });
                await context.SaveChangesAsync();
            }
        }

        if (!context.Tags.Any())
        {
            context.Tags.AddRange(
                new Tag { TagGroup = "company_type", TagName = "Client", TagValue = "clnt" },
                new Tag { TagGroup = "company_type", TagName = "Partner", TagValue = "part" },
                new Tag { TagGroup = "interaction_state", TagName = "Open", TagValue = "open" },
                new Tag { TagGroup = "interaction_state", TagName = "Closed", TagValue = "clsd" }
            );
            await context.SaveChangesAsync();
        }
    }
}
