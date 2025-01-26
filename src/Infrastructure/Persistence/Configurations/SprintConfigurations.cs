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