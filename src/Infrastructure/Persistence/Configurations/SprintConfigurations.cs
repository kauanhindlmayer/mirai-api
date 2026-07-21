using Domain.Sprints;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class SprintConfigurations : IEntityTypeConfiguration<Sprint>
{
    public void Configure(EntityTypeBuilder<Sprint> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .ValueGeneratedNever();

        builder.Property(s => s.TeamId)
            .IsRequired();

        builder.Property(s => s.Name)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(s => s.StartDate)
            .IsRequired();

        builder.Property(s => s.EndDate)
            .IsRequired();

        builder.Property(s => s.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(SprintStatus.Planned)
            .IsRequired();

        // Restated because declaring the filtered index below suppresses the
        // convention-generated one, and a partial index cannot serve the
        // unfiltered "sprints of this team" query every sprint list runs.
        builder.HasIndex(s => s.TeamId);

        // A team runs one sprint at a time. The domain enforces this too, but only
        // a unique index stops two concurrent starts from both winning. The name is
        // given twice on purpose: once to distinguish this index from the one above
        // in the model, once so the database name is not "ix_sprints_team_id1".
        builder.HasIndex(s => s.TeamId, "ix_sprints_team_id_active")
            .IsUnique()
            .HasFilter($"\"status\" = '{nameof(SprintStatus.Active)}'")
            .HasDatabaseName("ix_sprints_team_id_active");

        builder.HasOne(s => s.Team)
            .WithMany(t => t.Sprints)
            .HasForeignKey(s => s.TeamId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.WorkItems)
            .WithOne()
            .HasForeignKey(wi => wi.SprintId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}