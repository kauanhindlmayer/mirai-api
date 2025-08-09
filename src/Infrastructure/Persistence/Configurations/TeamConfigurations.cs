using Domain.Boards;
using Domain.Teams;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class TeamConfigurations : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .ValueGeneratedNever();

        builder.Property(t => t.Name)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(t => t.Description)
            .HasMaxLength(500);

        builder.Property(t => t.ProjectId)
            .IsRequired();

        builder.HasOne(t => t.Project)
            .WithMany(p => p.Teams)
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.Board)
            .WithOne(b => b.Team)
            .HasForeignKey<Board>(b => b.TeamId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.Sprints)
            .WithOne(s => s.Team)
            .HasForeignKey(s => s.TeamId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.Retrospectives)
            .WithOne(r => r.Team)
            .HasForeignKey(r => r.TeamId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.WorkItems)
            .WithOne(wi => wi.AssignedTeam)
            .HasForeignKey(wi => wi.AssignedTeamId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(t => t.Users)
            .WithMany(u => u.Teams)
            .UsingEntity<Dictionary<string, object>>(
                "TeamUsers",
                j => j.HasOne<User>()
                    .WithMany()
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<Team>()
                    .WithMany()
                    .HasForeignKey("TeamId")
                    .OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.HasKey("TeamId", "UserId");
                    j.ToTable("TeamUsers");
                    j.HasIndex("UserId");
                    j.HasIndex("TeamId");
                });

        builder.HasIndex(t => t.ProjectId);
        builder.HasIndex(t => new { t.ProjectId, t.Name })
            .IsUnique();
    }
}