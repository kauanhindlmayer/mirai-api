using Domain.Boards;
using Domain.Teams;
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
            .HasForeignKey(t => t.ProjectId);

        builder.HasOne(t => t.Board)
            .WithOne(b => b.Team)
            .HasForeignKey<Board>(b => b.TeamId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.Sprints)
            .WithOne(s => s.Team)
            .HasForeignKey(s => s.TeamId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}