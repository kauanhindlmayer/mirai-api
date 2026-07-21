using Domain.Sprints;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class SprintCommitmentConfigurations
    : IEntityTypeConfiguration<SprintCommitment>
{
    public void Configure(EntityTypeBuilder<SprintCommitment> builder)
    {
        builder.ToTable("sprint_commitments");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .ValueGeneratedNever();

        builder.Property(c => c.SprintId)
            .IsRequired();

        builder.Property(c => c.WorkItemId)
            .IsRequired();

        builder.Property(c => c.StatusAtStart)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(c => new { c.SprintId, c.WorkItemId })
            .IsUnique();
    }
}
