using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mirai.Domain.Retrospectives;

namespace Mirai.Infrastructure.Retrospectives.Persistence;

public class RetrospectiveConfigurations : IEntityTypeConfiguration<Retrospective>
{
    public void Configure(EntityTypeBuilder<Retrospective> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .ValueGeneratedNever();

        builder.Property(p => p.Title)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(p => p.Description)
            .HasMaxLength(500);

        builder.Property(p => p.TeamId)
            .IsRequired();

        builder.HasOne(p => p.Team)
            .WithMany(o => o.Retrospectives)
            .HasForeignKey(p => p.TeamId);

        builder.HasMany(p => p.Columns)
            .WithOne(c => c.Retrospective)
            .HasForeignKey(c => c.RetrospectiveId);
    }
}