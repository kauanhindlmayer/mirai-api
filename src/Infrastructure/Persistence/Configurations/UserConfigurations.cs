using Domain.Organizations;
using Domain.Projects;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class UserConfigurations : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .ValueGeneratedNever();

        builder.Property(u => u.FirstName)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(u => u.LastName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.Email)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(u => u.IdentityId)
            .IsRequired();

        builder.Property(u => u.ImageUrl)
            .HasMaxLength(512);

        builder.HasIndex(u => u.IdentityId)
            .IsUnique();

        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.HasMany(u => u.Organizations)
            .WithMany(o => o.Users)
            .UsingEntity(
                "OrganizationUsers",
                l => l.HasOne(typeof(Organization)).WithMany().HasForeignKey("OrganizationId"),
                r => r.HasOne(typeof(User)).WithMany().HasForeignKey("UserId"),
                j => j.HasKey("OrganizationId", "UserId"));

        builder.HasMany(u => u.Projects)
            .WithMany(p => p.Users)
            .UsingEntity(
                "ProjectUsers",
                l => l.HasOne(typeof(Project)).WithMany().HasForeignKey("ProjectId"),
                r => r.HasOne(typeof(User)).WithMany().HasForeignKey("UserId"),
                j => j.HasKey("ProjectId", "UserId"));

        builder.HasMany(u => u.WorkItems)
            .WithOne(w => w.Assignee)
            .HasForeignKey(w => w.AssigneeId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}