using Domain.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class RolePermissionConfigurations : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("role_permissions");

        builder.HasKey(rp => rp.Id);

        builder.Property(rp => rp.Id)
            .ValueGeneratedNever();

        builder.Property(rp => rp.Permission)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(rp => new { rp.RoleId, rp.Permission })
            .IsUnique();

        builder.HasData(SystemRoles.All.SelectMany(role => role.Permissions.Select(permission => new
        {
            permission.Id,
            permission.RoleId,
            permission.Permission,
            permission.CreatedAtUtc,
        })));
    }
}
