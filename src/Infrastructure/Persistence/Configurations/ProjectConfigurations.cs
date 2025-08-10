using Domain.Projects;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class ProjectConfigurations : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .ValueGeneratedNever();

        builder.Property(p => p.Name)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(p => p.Description)
            .HasMaxLength(500);

        builder.Property(p => p.OrganizationId)
            .IsRequired();

        builder.HasOne(p => p.Organization)
            .WithMany(o => o.Projects)
            .HasForeignKey(p => p.OrganizationId);

        builder.HasMany(p => p.WorkItems)
            .WithOne(w => w.Project)
            .HasForeignKey(w => w.ProjectId);

        builder.HasMany(p => p.Teams)
            .WithOne(t => t.Project)
            .HasForeignKey(t => t.ProjectId);

        builder.HasMany(p => p.Users)
            .WithMany(u => u.Projects)
            .UsingEntity<Dictionary<string, object>>(
                "project_users",
                j => j.HasOne<User>()
                    .WithMany()
                    .HasForeignKey("user_id")
                    .OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<Project>()
                    .WithMany()
                    .HasForeignKey("project_id")
                    .OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.HasKey("project_id", "user_id");
                    j.ToTable("project_users");
                    j.HasIndex("user_id");
                    j.HasIndex("project_id");
                });

        builder.HasMany(p => p.Tags)
            .WithOne(t => t.Project)
            .HasForeignKey(t => t.ProjectId);

        builder.HasMany(p => p.Personas)
            .WithOne(pe => pe.Project)
            .HasForeignKey(pe => pe.ProjectId);

        builder.HasMany(p => p.WikiPages)
            .WithOne(wp => wp.Project)
            .HasForeignKey(wp => wp.ProjectId);
    }
}