using Domain.Shared;

namespace Domain.Authorization;

public sealed class Role : AggregateRoot
{
    public string Name { get; private set; } = null!;
    public RoleScope Scope { get; private set; }
    public bool IsSystemRole { get; private set; }
    public ICollection<RolePermission> Permissions { get; private set; } = [];

    private Role()
    {
    }

    /// <summary>
    /// Fixed point in time used for every seeded system role/permission's
    /// <see cref="Entity.CreatedAtUtc"/>, instead of the non-deterministic
    /// <c>DateTime.UtcNow</c> default - otherwise EF would detect a "pending
    /// model change" on every process restart, since the HasData snapshot
    /// baked into the migration would never match freshly-computed values.
    /// </summary>
    private static readonly DateTime SeedCreatedAtUtc = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static Role CreateSystemRole(
        Guid id,
        string name,
        RoleScope scope,
        IEnumerable<Permission> permissions)
    {
        var role = new Role
        {
            Id = id,
            Name = name,
            Scope = scope,
            IsSystemRole = true,
            CreatedAtUtc = SeedCreatedAtUtc,
            Permissions = permissions
                .Select(permission => new RolePermission(
                    CreateDeterministicPermissionId(id, permission),
                    id,
                    permission,
                    SeedCreatedAtUtc))
                .ToList(),
        };

        return role;
    }

    public bool HasPermission(Permission permission)
    {
        return Permissions.Any(p => p.Permission == permission);
    }

    private static Guid CreateDeterministicPermissionId(Guid roleId, Permission permission)
    {
        var bytes = roleId.ToByteArray();
        var permissionBytes = BitConverter.GetBytes((int)permission);

        for (var i = 0; i < permissionBytes.Length; i++)
        {
            bytes[i] ^= permissionBytes[i];
        }

        return new Guid(bytes);
    }
}
