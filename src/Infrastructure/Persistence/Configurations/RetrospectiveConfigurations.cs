using Domain.Retrospectives;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class RetrospectiveConfigurations : IEntityTypeConfiguration<Retrospective>
{
    public void Configure(EntityTypeBuilder<Retrospective> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .ValueGeneratedNever();

        builder.Property(r => r.Title)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(r => r.Description)
            .HasMaxLength(500);

        builder.Property(r => r.TeamId)
            .IsRequired();

        builder.HasOne(r => r.Team)
            .WithMany(o => o.Retrospectives)
            .HasForeignKey(r => r.TeamId);

        builder.HasMany(r => r.Columns)
            .WithOne(c => c.Retrospective)
            .HasForeignKey(c => c.RetrospectiveId);
    }
}