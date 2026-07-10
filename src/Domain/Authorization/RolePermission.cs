using Domain.Shared;

namespace Domain.Authorization;

public sealed class RolePermission : Entity
{
    public Guid RoleId { get; private set; }
    public Permission Permission { get; private set; }

    public RolePermission(Guid roleId, Permission permission)
    {
        RoleId = roleId;
        Permission = permission;
    }

    internal RolePermission(Guid id, Guid roleId, Permission permission, DateTime createdAtUtc)
    {
        Id = id;
        RoleId = roleId;
        Permission = permission;
        CreatedAtUtc = createdAtUtc;
    }

    private RolePermission()
    {
    }
}
