using Domain.Organizations;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class OrganizationConfigurations : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Id)
            .ValueGeneratedNever();

        builder.Property(o => o.Name)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(o => o.Description)
            .HasMaxLength(500);

        builder.HasMany(o => o.Projects)
            .WithOne()
            .HasForeignKey(p => p.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(o => o.Users)
            .WithMany(u => u.Organizations)
            .UsingEntity<Dictionary<string, object>>(
                "OrganizationUsers",
                j => j.HasOne<User>()
                    .WithMany()
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<Organization>()
                    .WithMany()
                    .HasForeignKey("OrganizationId")
                    .OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.HasKey("OrganizationId", "UserId");
                    j.ToTable("OrganizationUsers");
                    j.HasIndex("UserId");
                    j.HasIndex("OrganizationId");
                });
    }
}