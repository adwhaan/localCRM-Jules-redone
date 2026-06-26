namespace LocalCRM.Domain.Entities;

public class Permission
{
    public int PermissionId { get; set; }
    public string PermissionName { get; set; } = string.Empty;

    public ICollection<Role> Roles { get; set; } = new List<Role>();
}
